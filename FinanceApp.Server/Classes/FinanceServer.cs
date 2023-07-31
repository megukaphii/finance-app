﻿using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Abstractions;
using FinanceApp.Data;
using FinanceApp.Server.Data;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IServer
{
	private readonly Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly int timeoutInMs = 60000;
	private readonly X509Certificate serverCertificate;
	private readonly IDatabase _database;

	private bool _isRunning = false;

	public FinanceServer(IDatabase database) {
		IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
		listener.Bind(ipEndPoint);
		serverCertificate = X509Certificate.CreateFromCertFile("./Resources/certificate.pfx");
		_database = database;
	}

	public async Task Start()
	{
		try
		{
			listener.Listen(10);
			Console.WriteLine("Listener started.");
			_isRunning = true;

			while (_isRunning) {
				Socket handle = await listener.AcceptAsync();
				Console.WriteLine("Connection found.");
				ThreadPool.QueueUserWorkItem(HandleNewConnection, handle);
			}
		} catch (Exception e) {
			Console.WriteLine($"[{e.GetType()}]: {e.Message}");
		}
	}

	private async void HandleNewConnection(object? handle) {
		Socket client = (Socket)handle!;
		using SslStream sslStream = await EstablishSslStream(client);
		while (_isRunning) {
			if (client.Poll(1, SelectMode.SelectRead) && client.Available == 0) {
				// TODO - I don't understand how this works, please explain!
				Console.WriteLine("Client has disconnected.");
				break;
			}

			string messageReceived = await ReadMessage(sslStream);

			if (messageReceived.StartsWith(CreateTransaction.GetFlag())) {
				messageReceived = messageReceived.Replace(CreateTransaction.GetFlag(), "");
				HandleCreateTransactionRequest(messageReceived, sslStream);
			}
		}
	}

	private async Task<SslStream> EstablishSslStream(Socket handler)
    {
		SslStream sslStream = GetSslStream(handler);
		sslStream = await SetupSslStream(sslStream);

		return sslStream;
	}

	private SslStream GetSslStream(Socket handler)
	{
        NetworkStream networkStream = new(handler);
        return new SslStream(networkStream, false);
	}

	private async Task<SslStream> SetupSslStream(SslStream sslStream)
	{
		await sslStream.AuthenticateAsServerAsync(serverCertificate, false, true);
		sslStream.ReadTimeout = timeoutInMs;
		sslStream.WriteTimeout = timeoutInMs;
		Console.WriteLine("SSL connection established.");

		return sslStream;
	}

	static async Task<string> ReadMessage(SslStream sslStream) {
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
		int bytes = -1;
		do {
			bytes = await sslStream.ReadAsync(buffer, 0, buffer.Length);

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

	private void HandleCreateTransactionRequest(string messageReceived, SslStream sslStream)
	{
        CreateTransaction? transactionRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateTransaction>(messageReceived);
		// TODO - transactionRequest.Handle(); ?

        if (transactionRequest == null)
        {
            Console.WriteLine("Transaction was null");
            return;
        }
        Console.WriteLine(transactionRequest);

		Transaction transaction = new(_database, transactionRequest.Value, "TEMP");
        transaction.Save();

        SendCreateTransactionResponse(sslStream, transaction);
    }

	private async void SendCreateTransactionResponse(SslStream sslStream, Transaction transaction)
	{
        CreateTransactionResponse response = new()
        {
            Id = transaction.ID,
            Success = true
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await sslStream.WriteAsync(message);
        sslStream.Flush();
    }

	static void DisplaySecurityLevel(SslStream stream) {
		Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
		Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
		Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
		Console.WriteLine("Protocol: {0}", stream.SslProtocol);
	}

	static void DisplaySecurityServices(SslStream stream) {
		Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
		Console.WriteLine("IsSigned: {0}", stream.IsSigned);
		Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
	}

	static void DisplayStreamProperties(SslStream stream) {
		Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
		Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
	}

	static void DisplayCertificateInformation(SslStream stream) {
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
