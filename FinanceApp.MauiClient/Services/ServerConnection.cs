using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Extensions;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using FinanceApp.Data.Requests.Transaction;

namespace FinanceApp.MauiClient.Services;
public class ServerConnection
{
    public const string DEFAULT_ADDRESS = "127.0.0.1";
    private const int READ_TIMEOUT = 10000;

    public bool IsConnected => _socket.Connected;

    private SslStream? _sslStream;
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public async Task<bool> EstablishConnection(string ipAddressStr = "")
    {
        if (string.IsNullOrEmpty(ipAddressStr)) ipAddressStr = DEFAULT_ADDRESS;

        await ConnectToIpAsync(ipAddressStr);
        await EstablishStreamAsync();
        bool isCompatible = await IsServerCompatible();
        if (!isCompatible) {
            await _socket.DisconnectAsync(false);
			_socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
        return isCompatible;
	}

    // TODO - Rework responses, figure out error handling within response
    public async Task<TResponse> SendMessageAsync<TRequest, TResponse>(TRequest request) where TRequest : IRequest
	{
        string json = JsonSerializer.Serialize(request);
		byte[] message = Encoding.UTF8.GetBytes(TRequest.Flag + json + "<EOF>");
        
        if (_sslStream == null) throw new InvalidOperationException("Cannot send a message without a valid SSL stream.");

		_sslStream.Write(message);
		_sslStream.Flush();

        // TODO - Handle no valid flag exists for and other errors resulting in disconnection
		string messageReceived = await ReadMessageAsync(_sslStream);
        return JsonSerializer.Deserialize<TResponse>(messageReceived) ??
            throw new($"Malformed {nameof(CreateResponse)} from server");
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

    private Task EstablishStreamAsync()
    {
        NetworkStream networkStream = new(_socket);
        _sslStream = new(networkStream, false, ValidateServerCertificate, null);
        return _sslStream.AuthenticateAsClientAsync("CoryMacdonald");
    }

    private static bool ValidateServerCertificate(
        object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        /*if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        // Do not allow this client to communicate with unauthenticated servers.
        return false;*/
        return true;
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
            byte[] message = Encoding.UTF8.GetBytes(strRequest + "<EOF>");
            await _sslStream.WriteAsync(message);
            await _sslStream.FlushAsync();

			string messageReceived = await ReadMessageAsync(_sslStream);
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

    private static async Task<string> ReadMessageAsync(Stream stream)
    {
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new();
        CancellationTokenSource source = new();
        bool readFirstBlock = false;
        do {
            if (readFirstBlock)
                source.CancelAfter(READ_TIMEOUT);

            int bytes = await stream.ReadAsync(buffer, source.Token);
            readFirstBlock = true;

            if (bytes <= 0) {
                // TODO - Handle disconnection!
            }

            messageData.Append(DecodeBuffer(buffer, bytes));
            if (messageData.ToString().Contains("<EOF>")) {
                break;
            } else {
                source.Dispose();
                source = new();
            }
        } while (true);

        source.Dispose();
        return messageData.ToString().Replace("<EOF>", "");
    }

    private static char[] DecodeBuffer(byte[] buffer, int bytes)
    {
        Decoder decoder = Encoding.UTF8.GetDecoder();
        char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
        decoder.GetChars(buffer, 0, bytes, chars, 0);
        return chars;
    }
}