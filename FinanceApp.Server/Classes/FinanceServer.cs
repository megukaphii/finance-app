using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FinanceApp.Server.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IHostedService
{
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
        (_serverCertificate, _cache, _maxConnections) = (GetCertificateFile(), cache, new(workerThreads));
    }

    private static X509Certificate GetCertificateFile()
    {
        return File.Exists("certificate.key")
            ? X509Certificate2.CreateFromPemFile("certificate.crt", "certificate.key")
            : X509Certificate2.CreateFromPemFile("/Certificates/certificate.crt", "/Certificates/certificate.key");
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
            client.WriteLine("Connection found.");
            await using SslStream sslStream = await client.EstablishSslStreamAsync(_serverCertificate);
            client.Stream = sslStream;
            ClientInitialiser initialiser = new(client);
            if (await initialiser.Initialise()) {
                while (_isRunning) {
                    string strRequest = await client.ReadMessageAsync();
                    if (string.IsNullOrWhiteSpace(strRequest)) break;

                    dynamic request = IRequest.GetRequest(strRequest);
                    client.WriteLine(request);
                    if (await IRequest.IsValidAsync(request, _db)) {
                        await request.HandleAsync(_db, client);
                    } else {
                        await SendErrorResponseAsync(client.Stream, request);
                    }
                }
            }
        } catch (Exception e) {
            client.WriteLine(e.Message);
		} finally {
            await RemoveClientAsync(client);
        }
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
        await client.Socket.DisconnectAsync(false);
        _clients.Remove(client);
        _maxConnections.Release();
        client.WriteLine("Client connection closed.");
    }
}