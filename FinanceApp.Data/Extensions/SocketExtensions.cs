using System.Net;
using System.Net.Sockets;

public static class SocketExtensions
{
	public static string GetIpStr(this Socket socket)
	{
		IPEndPoint? endpoint = (IPEndPoint?) socket.RemoteEndPoint;

		if (endpoint is null)
			return string.Empty;

		return endpoint.Address.ToString();
	}
}