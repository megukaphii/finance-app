using System.Net;
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

namespace FinanceApp.Server.Classes;

public class FinanceServer : IServer
{
    private const int READ_TIMEOUT = 10000;

    private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly X509Certificate _serverCertificate;
    private readonly List<SocketStream> _clients = new();
    private readonly FinanceAppContext _db = new();

    private bool _isRunning;

    public FinanceServer()
    {
        IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
        _listener.Bind(ipEndPoint);
        if (File.Exists("certificate.key")) {
            _serverCertificate = X509Certificate2.CreateFromPemFile("certificate.crt", "certificate.key");
        } else {
			_serverCertificate = X509Certificate2.CreateFromPemFile("/Certificates/certificate.crt", "/Certificates/certificate.key");
        }
    }

    public async Task Start()
    {
        await PerformMigrations();

        try {
            _listener.Listen(10);
            Console.WriteLine("Listener started.");
            _isRunning = true;

            while (_isRunning) {
                Socket handle = await _listener.AcceptAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				HandleConnection(handle);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}

            await CloseAsync();
        } catch (Exception e) {
            Console.WriteLine($"[{e.GetType()}]: {e.Message}");
        }
	}

	private async Task PerformMigrations()
	{
		if ((await _db.Database.GetPendingMigrationsAsync()).Any()) {
			await _db.Database.MigrateAsync();
		}
	}

	private async Task HandleConnection(Socket socket)
	{
        SocketStream client = new() { Socket = socket, Stream = Stream.Null };
        _clients.Add(client);
		try {
            Console.WriteLine($"[{socket.GetIpStr()}] Connection found.");
            using SslStream sslStream = await EstablishSslStreamAsync(socket);
            client.Stream = sslStream;
            if (await IsClientCompatibleAsync(client)) {
                while (_isRunning) {
                    string strRequest = await ReadMessageAsync(client);
                    if (strRequest.Equals(string.Empty)) break;

                    IRequest request = IRequest.GetRequest(strRequest);
                    if (IRequest.IsValid(request)) {
                        await request.HandleAsync(_db, client);
                    } else {
                        await SendErrorResponseAsync(sslStream, request);
                    }
                }
            }
        } catch (OperationCanceledException) {
			Console.WriteLine($"[{socket.GetIpStr()}] Client timed out.");
		} catch (Exception e) {
			Console.WriteLine($"[{socket.GetIpStr()}] {e}");
		} finally {
            await RemoveClientAsync(client);
        }
	}

    private async Task<SslStream> EstablishSslStreamAsync(Socket socket)
    {
        NetworkStream networkStream = new(socket);
        SslStream sslStream = new(networkStream, false);
		await sslStream.AuthenticateAsServerAsync(_serverCertificate, false, true);
		Console.WriteLine($"[{socket.GetIpStr()}] SSL connection established.");
        return sslStream;
	}

    private async Task<bool> IsClientCompatibleAsync(SocketStream client)
    {
        try {
            CompareVersion request = new()
            {
                SemanticVersion = ThisAssembly.Git.SemVer.Version
            };
            string strRequest = JsonSerializer.Serialize(request);

            byte[] message = Encoding.UTF8.GetBytes(strRequest + "<EOF>");
            await client.Stream.WriteAsync(message);
            await client.Stream.FlushAsync();

            string messageReceived = await ReadMessageAsync(client);
            CompareVersion response = JsonSerializer.Deserialize<CompareVersion>(messageReceived) ?? throw new Exception($"No {nameof(CompareVersion)} message received");

            if (response.SemanticVersion.IsCompatible(request.SemanticVersion)) {
                return true;
            } else {
                Console.WriteLine($"Client has incompatible version - {response.SemanticVersion}");
                return false;
            }
		} catch {
            Console.WriteLine($"Client did not send appropriate {nameof(CompareVersion)} request, disconnecting.");
            return false;
        }
	}

	private async Task<string> ReadMessageAsync(SocketStream client)
	{
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
        CancellationTokenSource source = new();
        bool readFirstBlock = false;
		int bytes;
		do {
            if (readFirstBlock)
                source.CancelAfter(READ_TIMEOUT);

			bytes = await client.Stream.ReadAsync(buffer, source.Token);
            readFirstBlock = true;

			if (bytes <= 0) {
                Console.WriteLine($"[{client.IPAddress}] Client disconnected.");
				await RemoveClientAsync(client);
                break;
            }

			messageData.Append(DecodeBuffer(buffer, bytes));
			if (messageData.ToString().Contains("<EOF>")) {
				break;
			} else {
                source.Dispose();
                source = new();
            }
		} while (bytes != 0);

        source.Dispose();
		return messageData.ToString().Replace("<EOF>", "");
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
        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await stream.WriteAsync(message);
        await stream.FlushAsync();
	}

	private async Task RemoveClientAsync(SocketStream client)
	{
        string clientIp = client.IPAddress;
		await client.Socket.DisconnectAsync(false);
        _clients.Remove(client);
		Console.WriteLine($"[{clientIp}] Client connection closed.");
	}

    private async Task CloseAsync()
    {
        foreach (SocketStream client in _clients) {
            await client.Socket.DisconnectAsync(false);
        }

        _clients.Clear();
    }
}