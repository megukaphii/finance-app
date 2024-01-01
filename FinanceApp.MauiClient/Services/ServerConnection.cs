using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;
using FinanceApp.MauiClient.Classes;
using FinanceApp.MauiClient.Extensions;

namespace FinanceApp.MauiClient.Services;

public class ServerConnection
{
	public const string DefaultAddress = "127.0.0.1";
	private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private SslStream? _sslStream;

	public bool IsConnected => _socket.Connected;

	public async Task<bool> EstablishConnection(string ipAddressStr = "")
	{
		if (string.IsNullOrEmpty(ipAddressStr)) ipAddressStr = DefaultAddress;

		await ConnectToIpAsync(ipAddressStr);
		_sslStream = await _socket.EstablishSslStreamAsync();
		ServerInitialiser initialiser = new(this);
		bool success = await initialiser.Initialise();
		if (!success) {
			await _socket.DisconnectAsync(false);
			_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		return success;
	}

	public async Task<TResponse> SendMessageAsync<TRequest, TResponse>(TRequest request)
		where TRequest : IRequest where TResponse : IResponse
	{
		if (_sslStream == null)
			throw new InvalidOperationException("Cannot send a message without a valid SSL stream.");

		await _sslStream.SendRequestAsync(request);

		string messageReceived = await _sslStream.ReadMessageAsync();
		if (messageReceived.Contains(Serialization.Error)) {
			messageReceived = messageReceived.Replace(Serialization.Error, "");
			TRequest errorResponse = Serialization.Deserialize<TRequest>(messageReceived) ??
			                         throw new($"Malformed <{typeof(TRequest).Name}> from server");
			throw new ResponseException<TRequest> { Response = errorResponse };
		} else {
			return Serialization.Deserialize<TResponse>(messageReceived) ??
			       throw new($"Malformed <{typeof(TResponse).Name}> from server");
		}
	}

	public async Task DisconnectAsync()
	{
		await _socket.DisconnectAsync(false);
		_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	private async Task ConnectToIpAsync(string ipAddressStr)
	{
		IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddressStr);
		IPAddress ip = hostEntry.AddressList[0] ?? throw new($"Unable to find IP address for {ipAddressStr}");
		IPEndPoint ipEndPoint = new(ip, 42069);
		await _socket.ConnectAsync(ipEndPoint);
	}
}