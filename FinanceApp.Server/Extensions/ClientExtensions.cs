using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Extensions;

public static class ClientExtensions
{
	public static Task<SslStream> EstablishSslStreamAsync(this Client client, X509Certificate certificate) =>
		client.Socket.EstablishSslStreamAsync(certificate, client);
}