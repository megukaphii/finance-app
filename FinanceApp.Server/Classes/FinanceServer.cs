using FinanceApp.Abstractions;
using FinanceApp.Data;
using FinanceApp.Extensions.Sqlite;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server.Classes;

public class FinanceServer : IServer
{
	private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private X509Certificate? serverCertificate = null;

	public async Task Start()
	{
		serverCertificate = X509Certificate.CreateFromCertFile("./Resources/certificate.pfx");

		IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 42069);

		try
		{
			Console.WriteLine("Starting listener...");
			listener.Bind(ipEndPoint);
			listener.Listen(1);

			Socket handler = await listener.AcceptAsync();
			Console.WriteLine("Connection found.");
			using NetworkStream networkStream = new NetworkStream(handler);
			Console.WriteLine(handler.Connected);
			SslStream sslStream = new SslStream(networkStream, false);
			sslStream.AuthenticateAsServer(serverCertificate, false, true);

			sslStream.ReadTimeout = 1000000;
			sslStream.WriteTimeout = 1000000;

			string messageReceived = ReadMessage(sslStream);
			CreateTransaction? transaction = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateTransaction>(messageReceived);
			Console.WriteLine(transaction);

			if (transaction == null)
			{
				Console.WriteLine("oh no the transaction is null");
				handler.Close();
				return;
			}

			using (SqliteDatabase db = new()) {
				string sql =
				@"
					INSERT INTO Transactions (
						Value
					)
					VALUES (
						$value
					);
				";
				ParameterCollection parameters = new() {
					new Parameter(System.Data.SqlDbType.Int, "$value", transaction.Value)
				};

				int rowsUpdated = db.ExecuteNonQuery(sql, parameters);
				long newId = db.LastInsertId ?? -1;

				CreateTransactionResponse response = new() {
					Id = newId,
					Success = true
				};
				string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

				byte[] messsage = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
				sslStream.Write(messsage);
				sslStream.Flush();
			}

			// write message

			/*while (true)
			{
				byte[] messageReceived = new byte[1024];
				int numByte = await handler.ReceiveAsync(messageReceived);
				string messageAsStr = Encoding.UTF8.GetString(messageReceived, 0, numByte);
				Console.WriteLine("Message from Client -> {0}", messageAsStr);

				byte[] messageSent = Encoding.UTF8.GetBytes(messageAsStr + " reply!");
				handler.Send(messageSent);
			}

			handler.Shutdown(SocketShutdown.Both);
			handler.Close();*/
		} catch (Exception e)
		{
			Console.WriteLine($"[{e.GetType()}]: {e.Message}");
		}
	}

	static string ReadMessage(SslStream sslStream) {
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new StringBuilder();
		int bytes = -1;
		do {
			bytes = sslStream.Read(buffer, 0, buffer.Length);

			Decoder decoder = Encoding.UTF8.GetDecoder();
			char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
			decoder.GetChars(buffer, 0, bytes, chars, 0);
			messageData.Append(chars);
			if (messageData.ToString().IndexOf("<EOF>") != -1) {
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
