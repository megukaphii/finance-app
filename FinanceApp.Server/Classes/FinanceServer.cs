using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
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
    private readonly List<Socket> _sockets = new();
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
        LoadAssemblies();

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

    private async Task HandleConnection(Socket socket)
	{
		_sockets.Add(socket);
		try {
            Console.WriteLine($"[{socket.GetIpStr()}] Connection found.");
            using SslStream sslStream = await EstablishSslStream(socket);
            if (await IsClientCompatible(sslStream)) {
                while (_isRunning) {
                    string strRequest = await ReadMessage(sslStream);

                    IRequest request = IRequest.GetRequest(strRequest);
                    if (request.IsValid()) {
                        await request.Handle(_db, sslStream);
                    } else {
                        await SendErrorResponse(sslStream, request);
                    }
                }
            } else {
				await RemoveClient(socket);
            }
        } catch (Exception e) {
			Console.WriteLine($"[{socket.GetIpStr()}] {e}");
			await RemoveClient(socket);
		}
	}

    private async Task PerformMigrations()
    {
        if ((await _db.Database.GetPendingMigrationsAsync()).Any()) {
            await _db.Database.MigrateAsync();
        }
    }

    private static void LoadAssemblies()
    {
        Assembly.Load("FinanceApp.Data");
    }

    private async Task<SslStream> EstablishSslStream(Socket socket)
    {
        SslStream sslStream = GetSslStream(socket);
        return await SetupSslStream(sslStream);
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
        Console.WriteLine("SSL connection established.");

		DisplaySslInfo(sslStream);

		return sslStream;
    }

    private async Task<bool> IsClientCompatible(Stream stream)
    {
        try {
            CompareVersion request = new()
            {
                SemanticVersion = ThisAssembly.Git.SemVer.Version
            };
            string strRequest = JsonConvert.SerializeObject(request);

            byte[] message = Encoding.UTF8.GetBytes(strRequest + "<EOF>");
            await stream.WriteAsync(message);
            await stream.FlushAsync();

            string messageReceived = await ReadMessage(stream);
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

    private static async Task<string> ReadMessage(Stream stream)
    {
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new();
        int bytes;
        do {
            bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

            // TODO - Deal with unexpected disconnection?

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

	private async Task RemoveClient(Socket socket)
	{
        string socketIp = socket.GetIpStr();
        await socket.DisconnectAsync(false);
		_sockets.Remove(socket);
		Console.WriteLine($"[{socketIp}] Client connection closed.");
	}

    private async Task Close()
    {
        foreach (Socket socket in _sockets) {
            await socket.DisconnectAsync(false);
        }

        _sockets.Clear();
    }

    static void DisplaySslInfo(SslStream stream)
    {
		Console.WriteLine("------START OF SSL INFO--------");
		DisplaySecurityLevel(stream);
		DisplaySecurityServices(stream);
		DisplayStreamProperties(stream);
        DisplayCertificateInformation(stream);
		Console.WriteLine("------END OF SSL INFO--------");
	}

    static void DisplaySecurityLevel(SslStream stream)
    {
        Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
        Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
        Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
        Console.WriteLine("Protocol: {0}", stream.SslProtocol);
    }

    static void DisplaySecurityServices(SslStream stream)
    {
        Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
        Console.WriteLine("IsSigned: {0}", stream.IsSigned);
        Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
    }

    static void DisplayStreamProperties(SslStream stream)
    {
        Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
        Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
    }

    static void DisplayCertificateInformation(SslStream stream)
    {
        Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

        X509Certificate? localCertificate = stream.LocalCertificate;
        if (localCertificate is not null) {
            Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                localCertificate.Subject,
                localCertificate.GetEffectiveDateString(),
                localCertificate.GetExpirationDateString());
        } else {
            Console.WriteLine("Local certificate is null.");
        }

        // Display the properties of the client's certificate.
        X509Certificate? remoteCertificate = stream.RemoteCertificate;
        if (remoteCertificate is not null) {
            Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                remoteCertificate.Subject,
                remoteCertificate.GetEffectiveDateString(),
                remoteCertificate.GetExpirationDateString());
        } else {
            Console.WriteLine("Remote certificate is null.");
        }
    }
}