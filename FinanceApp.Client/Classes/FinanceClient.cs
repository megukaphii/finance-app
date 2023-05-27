using FinanceApp.Data;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Client.Classes;

public class FinanceClient : IClient
{
	private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public static bool ValidateServerCertificate(
			  object sender,
			  X509Certificate? certificate,
			  X509Chain? chain,
			  SslPolicyErrors sslPolicyErrors) {
		/*if (sslPolicyErrors == SslPolicyErrors.None)
			return true;

		Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

		// Do not allow this client to communicate with unauthenticated servers.
		return false;*/
		return true;
	}

	public async Task Start()
	{
		Console.WriteLine("Please enter IP address!");

		string ipStr = Console.ReadLine() ?? "";
		if (ipStr == "") {
			ipStr = "127.0.0.1";
		}

		IPAddress ip = IPAddress.Parse(ipStr);
		IPEndPoint ipEndPoint = new IPEndPoint(ip, 42069);

		try
		{
			await client.ConnectAsync(ipEndPoint);
			Console.WriteLine("Connected!");

			NetworkStream networkStream = new(client);
			SslStream sslStream = new SslStream(
				networkStream,
				false,
				new RemoteCertificateValidationCallback(ValidateServerCertificate),
				null
			);

			sslStream.AuthenticateAsClient("Cory Macdonald");

			while (true) {
				int value = int.Parse(Console.ReadLine() ?? "0");

				if (value > 0) {
					CreateTransaction transaction = new() {
						Type = "create",
						Value = value
					};

					string json = Newtonsoft.Json.JsonConvert.SerializeObject(transaction);

					byte[] messsage = Encoding.UTF8.GetBytes(json + "<EOF>");
					sslStream.Write(messsage);
					sslStream.Flush();

					string messageReceived = await ReadMessage(sslStream);
					CreateTransactionResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateTransactionResponse>(messageReceived);
					Console.WriteLine(response);
				}
			}

			client.Close();
			Console.WriteLine("Client closed.");
		} catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	static async Task<string> ReadMessage(SslStream sslStream) {
		byte[] buffer = new byte[2048];
		StringBuilder messageData = new StringBuilder();
		int bytes = -1;
		do {
			bytes = await sslStream.ReadAsync(buffer, 0, buffer.Length);

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
}
