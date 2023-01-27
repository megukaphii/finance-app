using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client.Classes;

public class FinanceClient : IClient
{
	private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public async Task Start()
	{
		IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 42069);

		try
		{
			await client.ConnectAsync(ipEndPoint);
			Console.WriteLine(client.Connected);

			while (true)
			{
				await Task.Delay(3000);

				byte[] messageSent = Encoding.UTF8.GetBytes("10");
				client.Send(messageSent);

				byte[] messageReceived = new byte[1024];
				int numByte = await client.ReceiveAsync(messageReceived);
				string messageAsStr = Encoding.UTF8.GetString(messageReceived, 0, numByte);
				Console.WriteLine("Message from Server -> {0}", messageAsStr);
			}

			client.Shutdown(SocketShutdown.Both);
			client.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine("oh no");
		}
	}
}
