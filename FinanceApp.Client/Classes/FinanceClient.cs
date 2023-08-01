using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data;
using Newtonsoft.Json;

namespace FinanceApp.Client.Classes;

public class FinanceClient : IClient
{
	private readonly Socket _client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	private static bool ValidateServerCertificate(
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

		IPAddress ip = (await Dns.GetHostEntryAsync(ipStr)).AddressList[0] ?? throw new Exception($"Unable to find IP address for {ipStr}");
		IPEndPoint ipEndPoint = new (ip, 42069);

		try
		{
			// TODO - Crashes when ipStr is "localhost"
			await _client.ConnectAsync(ipEndPoint);
			Console.WriteLine("Connected!");

			NetworkStream networkStream = new(_client);
			SslStream sslStream = new(
				networkStream,
				false,
				ValidateServerCertificate,
				null
			);

			await sslStream.AuthenticateAsClientAsync("Cory Macdonald");

			while (true) {
				Console.WriteLine("Please enter a transaction value!");
				int value = int.Parse(Console.ReadLine() ?? "0");

				if (value > 0) {
					CreateTransaction transaction = new() {
						Type = "create",
						Value = value
					};

					string json = JsonConvert.SerializeObject(transaction);

					byte[] message = Encoding.UTF8.GetBytes(CreateTransaction.GetFlag() + json + "<EOF>");
					sslStream.Write(message);
					sslStream.Flush();

					string messageReceived = await ReadMessage(sslStream);
					CreateTransactionResponse? response = JsonConvert.DeserializeObject<CreateTransactionResponse>(messageReceived);
					Console.WriteLine(response);
				}
			}

			_client.Close();
			Console.WriteLine("Client closed.");
		} catch (Exception e)
		{
			Console.WriteLine(e);
		}
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
}
