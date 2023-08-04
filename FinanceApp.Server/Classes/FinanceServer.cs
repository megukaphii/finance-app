using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Abstractions;

namespace FinanceApp.Server.Classes;

public class FinanceServer : IServer
{
	private const int TimeoutInMs = 60000;
	private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private readonly X509Certificate _serverCertificate;
	private readonly IDatabase _database;

	private bool _isRunning;

	public FinanceServer(IDatabase database) {
		IPEndPoint ipEndPoint = new(IPAddress.Any, 42069);
		_listener.Bind(ipEndPoint);
		_serverCertificate = X509Certificate.CreateFromCertFile("./Resources/certificate.pfx");
		_database = database;
	}

	public async Task Start()
	{
		LoadAssemblies();

		try
		{
			_listener.Listen(10);
			Console.WriteLine("Listener started.");
			_isRunning = true;

			while (_isRunning) {
				Socket handle = await _listener.AcceptAsync();
				Console.WriteLine("Connection found.");
				await HandleNewConnection(handle);
			}
		} catch (Exception e) {
			Console.WriteLine($"[{e.GetType()}]: {e.Message}");
		}
	}

	private void LoadAssemblies()
	{
		Assembly.Load("FinanceApp.Data");
	}

	private async Task HandleNewConnection(Socket client)
	{
		await using SslStream sslStream = await EstablishSslStream(client);
		while (_isRunning) {
			if (client.Poll(1, SelectMode.SelectRead) && client.Available == 0) {
				Console.WriteLine("Client has disconnected.");
				break;
			}

			string message = await ReadMessage(sslStream);
			IRequest request = IRequest.GetRequest(message);
			await request.Handle(_database, sslStream);
		}
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

	private static async Task<string> ReadMessage(Stream sslStream) {
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new();
		int bytes;
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
