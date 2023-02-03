using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Classes;

public class FinanceServer : IServer
{
	private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public async Task Start()
	{
		IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 42069);

		try
		{
			/*using (var connection = new SqliteConnection("Data Source=testdb.db")) {
				connection.Open();

				var command = connection.CreateCommand();
				command.CommandText =
				@"
					CREATE TABLE Transactions (
						TransactionID int,
						Value int
					);
				";
				command.ExecuteNonQuery();
			}*/

			listener.Bind(ipEndPoint);

			listener.Listen(1);

			Socket handler = await listener.AcceptAsync();
			Console.WriteLine(handler.Connected);

			while (true)
			{
				byte[] messageReceived = new byte[1024];
				int numByte = await handler.ReceiveAsync(messageReceived);
				string messageAsStr = Encoding.UTF8.GetString(messageReceived, 0, numByte);
				Console.WriteLine("Message from Server -> {0}", messageAsStr);

				byte[] messageSent = Encoding.UTF8.GetBytes(messageAsStr + " reply!");
				handler.Send(messageSent);
			}

			/*await Task.Delay(3000);

			byte[] message = Encoding.UTF8.GetBytes("$800");
			handler.Send(message);*/

			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		} catch (Exception e)
		{
			Console.WriteLine("oh no");
		}
	}
}
