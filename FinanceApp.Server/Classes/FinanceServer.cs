﻿using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using FinanceApp.Data;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IHostedService
{
	private readonly IMemoryCache _cache;
	private readonly List<Client> _clients = new();
	private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private readonly SemaphoreSlim _maxConnections;
	private readonly IRequestProcessor _processor;
	private readonly X509Certificate _serverCertificate;

	private bool _isRunning;

	public FinanceServer(IRequestProcessor processor, IMemoryCache cache)
	{
		IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
		ThreadPool.GetMaxThreads(out int workerThreads, out int _);
		_listener.Bind(ipEndPoint);
		(_serverCertificate, _processor, _maxConnections, _cache) =
			(GetCertificateFile(), processor, new(workerThreads), cache);
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
			Console.WriteLine($"[{e.GetType().Name}] {e.Message}");
		}
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_isRunning = false;

		foreach (Client client in _clients) await client.Socket.DisconnectAsync(false, cancellationToken);

		_clients.Clear();
		_listener.Dispose();
		_maxConnections.Dispose();
	}

	private static X509Certificate GetCertificateFile() =>
		File.Exists("certificate.key")
			? X509Certificate2.CreateFromPemFile("certificate.crt", "certificate.key")
			: X509Certificate2.CreateFromPemFile("/Certificates/certificate.crt",
				"/Certificates/certificate.key");

	private static async Task PerformMigrations(CancellationToken cancellationToken)
	{
		await using FinanceAppContext context = new();
		List<string> migrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
		if (migrations.Any()) {
			Console.WriteLine($"Migrations to be applied: {string.Join(", ", migrations)}");
			await context.Database.MigrateAsync(cancellationToken);
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
			Console.WriteLine($"[{e.GetType().Name}] {e.Message}");
		}
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
			if (await initialiser.Initialise())
				while (_isRunning) {
					string strRequest = await client.ReadMessageAsync();
					if (string.IsNullOrWhiteSpace(strRequest)) break;

					dynamic request = IRequest.GetRequest(strRequest);
					client.WriteLine(request);
					if (await IRequest.IsValidAsync(request))
						await _processor.ProcessAsync(request, client);
					else
						await SendErrorResponseAsync(client.Stream, request);
				}
		} catch (ConnectionException e) {
			client.WriteLine(e.Message);
		} catch (Exception e) {
			string message = $"[{e.GetType().Name}] {e.Message}\n{e.StackTrace}";
			if (e.InnerException is not null)
				message +=
					$"\nInner Exception: [{e.InnerException.GetType().Name} {e.InnerException.Message}\n{e.InnerException.StackTrace}";

			client.WriteLine(message);
		} finally {
			await RemoveClientAsync(client);
		}
	}

	private static async Task SendErrorResponseAsync<TRequest>(Stream stream, TRequest validatedRequest)
		where TRequest : IRequest
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