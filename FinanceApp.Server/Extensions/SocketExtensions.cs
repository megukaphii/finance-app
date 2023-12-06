using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using FinanceApp.Data.Utility;

namespace FinanceApp.Server.Extensions;

public static class SocketExtensions
{
	public static async Task<SslStream> EstablishSslStreamAsync(this Socket socket, X509Certificate certificate,
		Client client)
	{
		NetworkStream networkStream = new(socket);
		SslStream sslStream = new(networkStream, false);
		await sslStream.AuthenticateAsServerAsync(certificate, false, true);
		client.WriteLine("SSL connection established.");
		return sslStream;
	}
}