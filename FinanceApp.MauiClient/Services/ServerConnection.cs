using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Extensions;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Utility;
using FinanceApp.MauiClient.Extensions;

namespace FinanceApp.MauiClient.Services;
public class ServerConnection
{
    public const string DefaultAddress = "127.0.0.1";

    public bool IsConnected => _socket.Connected;

    private SslStream? _sslStream;
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public async Task<bool> EstablishConnection(string ipAddressStr = "")
    {
        if (string.IsNullOrEmpty(ipAddressStr)) ipAddressStr = DefaultAddress;

        await ConnectToIpAsync(ipAddressStr);
        _sslStream = await _socket.EstablishSslStreamAsync();
        bool isCompatible = await IsServerCompatible();
        if (!isCompatible) {
            await _socket.DisconnectAsync(false);
			_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
        return isCompatible;
	}

    public async Task<TResponse> SendMessageAsync<TRequest, TResponse>(TRequest request) where TRequest : IRequest
	{
        string json = JsonSerializer.Serialize(request);
		byte[] message = Encoding.UTF8.GetBytes(TRequest.Flag + json + Serialization.Eof);

        if (_sslStream == null) throw new InvalidOperationException("Cannot send a message without a valid SSL stream.");

		_sslStream.Write(message);
		_sslStream.Flush();

		string messageReceived = await _sslStream.ReadMessageAsync();
        if (messageReceived.Contains(Serialization.Error)) {
            messageReceived = messageReceived.Replace(Serialization.Error, "");
            TRequest errorResponse = JsonSerializer.Deserialize<TRequest>(messageReceived) ??
                                     throw new($"Malformed {typeof(TRequest).Name} from server");
            throw new ResponseException<TRequest> { Response = errorResponse };
        } else {
            return JsonSerializer.Deserialize<TResponse>(messageReceived) ?? throw new($"Malformed {typeof(TResponse).Name} from server");
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

    private async Task<bool> IsServerCompatible()
    {
        try {
            if (_sslStream == null) throw new InvalidOperationException("Cannot send a message without a valid SSL stream.");

            CompareVersion response = new()
            {
                SemanticVersion = AppInfo.Version
            };

            string strRequest = JsonSerializer.Serialize(response);
            byte[] message = Encoding.UTF8.GetBytes(strRequest + Serialization.Eof);
            await _sslStream.WriteAsync(message);
            await _sslStream.FlushAsync();

			string messageReceived = await _sslStream.ReadMessageAsync();
            CompareVersion request = JsonSerializer.Deserialize<CompareVersion>(messageReceived) ?? throw new($"Malformed {nameof(CompareVersion)} request received");

			if (request.SemanticVersion.IsCompatible(AppInfo.Version)) {
				return true;
			} else {
				await Shell.Current.DisplayAlert("Version Issue", $"Server version {request.SemanticVersion} is not compatible with {response.SemanticVersion}", "OK");
				return false;
            }
		} catch {
			await Shell.Current.DisplayAlert("Version Issue", "Could not compare version against server.", "OK");
			return false;
        }
    }
}