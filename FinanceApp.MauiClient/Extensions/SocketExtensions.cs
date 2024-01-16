using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace FinanceApp.MauiClient.Extensions;

public static class SocketExtensions
{
	public static async Task<SslStream> EstablishSslStreamAsync(this Socket socket)
	{
		NetworkStream networkStream = new(socket);
		SslStream sslStream = new(networkStream, false, ValidateServerCertificate, null);
		await sslStream.AuthenticateAsClientAsync("CoryMacdonald");
		return sslStream;
	}

	private static bool ValidateServerCertificate(
		object sender,
		X509Certificate? certificate,
		X509Chain? chain,
		SslPolicyErrors sslPolicyErrors) =>
		/*if (sslPolicyErrors == SslPolicyErrors.None)
		    return true;

		Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

		// Do not allow this client to communicate with unauthenticated servers.
		return false;*/
		true;
}