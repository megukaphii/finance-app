using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

				byte[] messageSent = Encoding.UTF8.GetBytes("Hello, world!<EOF>");
				client.Send(messageSent);

				byte[] messageReceived = new byte[1024];
				int numByte = await client.ReceiveAsync(messageReceived);
				string messageAsStr = Encoding.UTF8.GetString(messageReceived, 0, numByte);
				Console.WriteLine("Message from Server -> {0}", messageAsStr);
			}

			/*await Task.Delay(3000);

			byte[] messageSent2 = Encoding.UTF8.GetBytes("Hi, again!<EOF>");
			client.Send(messageSent2);

			await Task.Delay(3000);

			byte[] messageSent3 = Encoding.UTF8.GetBytes("Wow!<EOF>");
			client.Send(messageSent3);

			byte[] messageReceived = new byte[1024];
			int byteRecv = await client.ReceiveAsync(messageReceived);
			Console.WriteLine("Message from Server -> {0}", Encoding.UTF8.GetString(messageReceived, 0, byteRecv));
*/
			client.Shutdown(SocketShutdown.Both);
			client.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine("oh no");
		}
	}
}
