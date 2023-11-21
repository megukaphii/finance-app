﻿using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FinanceApp.Data.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IHostedService
{
    private const int ReadTimeout = 10000;

    private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly X509Certificate _serverCertificate;
    private readonly List<Client> _clients = new();
    private readonly FinanceAppContext _db = new();
    private readonly SemaphoreSlim _maxConnections;
    private readonly IMemoryCache _cache;

    private bool _isRunning;

    public FinanceServer(IMemoryCache cache)
    {
        IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
        ThreadPool.GetMaxThreads(out int workerThreads, out int _);
        _listener.Bind(ipEndPoint);
        _serverCertificate = File.Exists("certificate.key")
            ? X509Certificate2.CreateFromPemFile("certificate.crt", "certificate.key")
            : X509Certificate2.CreateFromPemFile("/Certificates/certificate.crt", "/Certificates/certificate.key");
        _cache = cache;
        _maxConnections = new(workerThreads);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try {
            await PerformMigrations(cancellationToken);

            _listener.Listen(10);
            Console.WriteLine("Listener started.");
            _isRunning = true;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RunServer(cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        } catch (Exception e) {
            Console.WriteLine($"[{e.GetType()}]: {e.Message}");
        }
    }

    private async Task PerformMigrations(CancellationToken cancellationToken)
    {
        List<string> migrations = (await _db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (migrations.Any()) {
            Console.WriteLine($"Migrations to be applied: {string.Join(", ", migrations)}");
            await _db.Database.MigrateAsync(cancellationToken);
            Console.WriteLine("Migrations successfully applied!");
        }
    }

    private async Task RunServer(CancellationToken cancellationToken)
    {
        try {
            while (_isRunning) {
                await _maxConnections.WaitAsync(cancellationToken);
                Socket handle = await _listener.AcceptAsync(cancellationToken);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                HandleConnection(handle);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        } catch (Exception e) {
            Console.WriteLine($"[{e.GetType()}]: {e.Message}");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _isRunning = false;

        foreach (Client client in _clients) {
            await client.Socket.DisconnectAsync(false, cancellationToken);
        }
        _clients.Clear();
        _listener.Dispose();
        _maxConnections.Dispose();
    }

	private async Task HandleConnection(Socket socket)
    {
        Client client = new() { Socket = socket, Stream = Stream.Null };
        _clients.Add(client);
        try {
            Console.WriteLine($"[{client.Id}] Connection found.");
            await using SslStream sslStream = await EstablishSslStreamAsync(client);
            client.Stream = sslStream;
            if (await IsClientCompatibleAsync(client)) {
                while (_isRunning) {
                    string strRequest = await ReadMessageAsync(client);
                    if (strRequest.Equals(string.Empty)) break;

                    dynamic request = IRequest.GetRequest(strRequest);
                    Console.WriteLine($"[{client.Id}] {request}");
                    if (await IRequest.IsValidAsync(request, _db)) {
                        await request.HandleAsync(_db, client);
                    } else {
                        await SendErrorResponseAsync(sslStream, request);
                    }
                }
            }
        } catch (OperationCanceledException) {
            Console.WriteLine($"[{client.Id}] Client timed out.");
        } catch (ConnectionException e) {
            Console.WriteLine(e.Message);
        } catch (Exception e) {
			Console.WriteLine($"[{client.Id}] {e}");
		} finally {
            await RemoveClientAsync(client);
        }
	}

    private async Task<SslStream> EstablishSslStreamAsync(Client client)
    {
        NetworkStream networkStream = new(client.Socket);
        SslStream sslStream = new(networkStream, false);
		await sslStream.AuthenticateAsServerAsync(_serverCertificate, false, true);
		Console.WriteLine($"[{client.Id}] SSL connection established.");
        return sslStream;
	}

    private async Task<bool> IsClientCompatibleAsync(Client client)
    {
        try {
            string messageReceived = await ReadMessageAsync(client);
            CompareVersion response = JsonSerializer.Deserialize<CompareVersion>(messageReceived) ?? throw new($"Malformed {nameof(CompareVersion)} request received");

            CompareVersion request = new()
            {
                SemanticVersion = ThisAssembly.Git.SemVer.Version
            };
            string strRequest = JsonSerializer.Serialize(request);
            byte[] message = Encoding.UTF8.GetBytes(strRequest + Serialization.Eof);
            await client.Stream.WriteAsync(message);
            await client.Stream.FlushAsync();

            if (response.SemanticVersion.IsCompatible(request.SemanticVersion)) {
                return true;
            } else {
                Console.WriteLine($"[{client.Id}] Client has incompatible version - {response.SemanticVersion}");
                return false;
            }
		} catch {
            Console.WriteLine($"[{client.Id}] Client did not send appropriate {nameof(CompareVersion)} request, disconnecting.");
            return false;
        }
	}

	private async Task<string> ReadMessageAsync(Client client)
	{
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
        CancellationTokenSource source = new();
        bool readFirstBlock = false;
		do {
            if (readFirstBlock)
                source.CancelAfter(ReadTimeout);

			int bytes = await client.Stream.ReadAsync(buffer, source.Token);
            readFirstBlock = true;

			if (bytes <= 0) {
                throw new ConnectionException($"[{client.Id}] Client disconnected.");
            }

			messageData.Append(DecodeBuffer(buffer, bytes));
			if (messageData.ToString().Contains(Serialization.Eof)) {
				break;
			} else {
                source.Dispose();
                source = new();
            }
		} while (true);

        source.Dispose();
		return messageData.ToString().Replace(Serialization.Eof, "");
	}

    private static char[] DecodeBuffer(byte[] buffer, int bytes)
    {
		Decoder decoder = Encoding.UTF8.GetDecoder();
		char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
		decoder.GetChars(buffer, 0, bytes, chars, 0);
        return chars;
	}

    private static async Task SendErrorResponseAsync<TRequest>(Stream stream, TRequest validatedRequest) where TRequest : IRequest
    {
        string strResponse = JsonSerializer.Serialize(validatedRequest);
        byte[] message = Encoding.UTF8.GetBytes(Serialization.Error + strResponse + Serialization.Eof);
        await stream.WriteAsync(message);
        await stream.FlushAsync();
	}

	private async Task RemoveClientAsync(Client client)
	{
        string clientId = client.Id;
        await client.Socket.DisconnectAsync(false);
        _clients.Remove(client);
        _maxConnections.Release();
        Console.WriteLine($"[{clientId}] Client connection closed.");
    }
}