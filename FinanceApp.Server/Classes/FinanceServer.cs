using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IServer
{
    private const int TimeoutInMs = 60000;
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
                HandleConnection(handle);
            }

            await Close();
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
            using SslStream sslStream = await EstablishSslStream(socket);
            client.Stream = sslStream;
            if (await IsClientCompatible(client)) {
                while (_isRunning) {
                    string strRequest = await ReadMessage(client);
                    if (strRequest.Equals(string.Empty)) break;

                    IRequest request = IRequest.GetRequest(strRequest);
                    if (request.IsValid()) {
                        await request.Handle(_db, sslStream);
                    } else {
                        await SendErrorResponse(sslStream, request);
                    }
                }
            } else {
				await RemoveClient(client);
            }
        } catch (Exception e) {
			Console.WriteLine($"[{socket.GetIpStr()}] {e}");
			await RemoveClient(client);
		}
	}

    private async Task<SslStream> EstablishSslStream(Socket socket)
    {
        SslStream sslStream = GetSslStream(socket);
        SslStream result = await SetupSslStream(sslStream);
		Console.WriteLine($"[{socket.GetIpStr()}] SSL connection established.");
        return result;
	}

    private static SslStream GetSslStream(Socket socket)
    {
        NetworkStream networkStream = new(socket);
        return new SslStream(networkStream, false);
    }

    private async Task<SslStream> SetupSslStream(SslStream sslStream)
    {
        await sslStream.AuthenticateAsServerAsync(_serverCertificate, false, true);
        sslStream.ReadTimeout = TimeoutInMs;
        sslStream.WriteTimeout = TimeoutInMs;
		return sslStream;
    }

    private async Task<bool> IsClientCompatible(SocketStream client)
    {
        try {
            CompareVersion request = new()
            {
                SemanticVersion = ThisAssembly.Git.SemVer.Version
            };
            string strRequest = JsonConvert.SerializeObject(request);

            byte[] message = Encoding.UTF8.GetBytes(strRequest + "<EOF>");
            await client.Stream.WriteAsync(message);
            await client.Stream.FlushAsync();

            string messageReceived = await ReadMessage(client);
            CompareVersion response = JsonConvert.DeserializeObject<CompareVersion>(messageReceived) ?? throw new Exception($"No {nameof(CompareVersion)} message received");

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

	private async Task<string> ReadMessage(SocketStream client)
	{
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
		int bytes;
		do {
			bytes = await client.Stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytes <= 0) {
				await RemoveClient(client);
                break;
            }

			// TODO - Can we add a timeout or something? Because if a message has no <EOF> tag, the server will just hang here

			Decoder decoder = Encoding.UTF8.GetDecoder();
			char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
			decoder.GetChars(buffer, 0, bytes, chars, 0);
			messageData.Append(chars);
			if (messageData.ToString().Contains("<EOF>")) {
				break;
			}
		} while (bytes != 0);

		return messageData.ToString().Replace("<EOF>", "");
	}

    private static async Task SendErrorResponse(Stream stream, IRequest validatedRequest)
    {
        string strResponse = JsonConvert.SerializeObject(validatedRequest);
        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await stream.WriteAsync(message);
        await stream.FlushAsync();
	}

	private async Task RemoveClient(SocketStream client)
	{
        string clientIp = client.IPAddress;
		await client.Socket.DisconnectAsync(false);
        _clients.Remove(client);
		Console.WriteLine($"[{clientIp}] Client connection closed.");
	}

    private async Task Close()
    {
        foreach (SocketStream client in _clients) {
            await client.Socket.DisconnectAsync(false);
        }

        _clients.Clear();
    }
}