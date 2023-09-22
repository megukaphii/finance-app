﻿using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IServer
{
    private const int TimeoutInMs = 60000;
    private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly X509Certificate _serverCertificate;
    private readonly List<Stream> _clients = new();
    private readonly FinanceAppContext _db = new();

    private bool _isRunning;

    public FinanceServer()
    {
        IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
        _listener.Bind(ipEndPoint);
        if (File.Exists("../certificate.pfx")) {
            _serverCertificate = X509Certificate.CreateFromCertFile("../certificate.pfx");
        } else {
            _serverCertificate = X509Certificate.CreateFromCertFile("/app/certificate.pfx");
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
                Console.WriteLine("Connection found.");
                await using SslStream sslStream = await EstablishSslStream(handle);
                _clients.Add(sslStream);
                await HandleConnection(sslStream);
            }

            Close();
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

    private static void LoadAssemblies()
    {
        Assembly.Load("FinanceApp.Data");
    }

    private async Task<SslStream> EstablishSslStream(Socket handler)
    {
        SslStream sslStream = GetSslStream(handler);
        sslStream = await SetupSslStream(sslStream);

        return sslStream;
    }

    private static SslStream GetSslStream(Socket handler)
    {
        NetworkStream networkStream = new(handler);
        return new SslStream(networkStream, false);
    }

    private async Task<SslStream> SetupSslStream(SslStream sslStream)
    {
        await sslStream.AuthenticateAsServerAsync(_serverCertificate, false, true);
        sslStream.ReadTimeout = TimeoutInMs;
        sslStream.WriteTimeout = TimeoutInMs;
        Console.WriteLine("SSL connection established.");

        return sslStream;
    }

    private async Task HandleConnection(Stream stream)
    {
        try {
            while (_isRunning) {
                string strRequest = await ReadMessage(stream);

                IRequest request = IRequest.GetRequest(strRequest);
                if (request.IsValid()) {
                    await request.Handle(_db, stream);
                } else {
                    await SendErrorResponse(stream, request);
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e);
            RemoveClient(stream);
        }
    }

    private static async Task<string> ReadMessage(Stream stream)
    {
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new();
        int bytes;
        do {
            bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            messageData.Append(chars);
            if (messageData.ToString().IndexOf("<EOF>", StringComparison.Ordinal) != -1) {
                break;
            }
        } while (bytes != 0);

        return messageData.ToString().Replace("<EOF>", "");
    }

    private static async Task SendErrorResponse(Stream stream, IRequest validatedRequest)
    {
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(validatedRequest);
        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await stream.WriteAsync(message);
        await stream.FlushAsync();
    }

    private void RemoveClient(Stream stream)
    {
        stream.Close();
        _clients.Remove(stream);
        Console.WriteLine("Client connection closed.");
    }

    private void Close()
    {
        foreach (Stream stream in _clients) {
            stream.Close();
        }

        _clients.Clear();
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
        if (localCertificate != null) {
            Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                localCertificate.Subject,
                localCertificate.GetEffectiveDateString(),
                localCertificate.GetExpirationDateString());
        } else {
            Console.WriteLine("Local certificate is null.");
        }

        // Display the properties of the client's certificate.
        X509Certificate? remoteCertificate = stream.RemoteCertificate;
        if (remoteCertificate != null) {
            Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                remoteCertificate.Subject,
                remoteCertificate.GetEffectiveDateString(),
                remoteCertificate.GetExpirationDateString());
        } else {
            Console.WriteLine("Remote certificate is null.");
        }
    }
}