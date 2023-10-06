using System.Net;
using System.Net.Sockets;

namespace FinanceApp.Data.Extensions;

public static class SocketExtensions
{
	public static string GetIpStr(this Socket socket)
	{
		// TODO - Docker only returns the Gateway address for the container. Can we fix this?
		IPEndPoint? endpoint = (IPEndPoint?) socket.RemoteEndPoint;

		if (endpoint is null)
			return string.Empty;

		return endpoint.Address.ToString();
	}
}