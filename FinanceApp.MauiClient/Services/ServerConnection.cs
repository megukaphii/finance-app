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

    public bool IsConnected => _socket.Connected;

    private readonly SslStream _sslStream;
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public ServerConnection()
    {
        NetworkStream networkStream = new(_socket);
        _sslStream = new SslStream(networkStream, false, ValidateServerCertificate, null);
    }

    public async Task<bool> EstablishConnection(string ipAddressStr = "")
    {
        if (string.IsNullOrEmpty(ipAddressStr)) ipAddressStr = DEFAULT_ADDRESS;

        await ConnectToIPAsync(ipAddressStr);
        await EstablishStreamAsync();
        bool isCompatible = await IsServerCompatible();
        // TODO - Doesn't seem to actually wait for this? Should be more obvious in UI if it is waiting.
        // Introduce delay in FinanceServer.ReadMessage() to test this properly (or I guess in the compatible test thing server-side)
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
		_sslStream.Write(message);
		_sslStream.Flush();

        // TODO - Handle no valid flag exists for and other errors resulting in disconnection
		string messageReceived = await ReadMessageAsync(_sslStream);
        return JsonSerializer.Deserialize<TResponse>(messageReceived) ??
            throw new Exception($"Malformed {nameof(CreateResponse)} from server");
	}

    public async Task DisconnectAsync()
    {
        await _socket.DisconnectAsync(false);
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

    private async Task ConnectToIPAsync(string ipAddressStr)
    {
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddressStr);
        IPAddress ip = hostEntry.AddressList[0] ?? throw new Exception($"Unable to find IP address for {ipAddressStr}");
        IPEndPoint ipEndPoint = new(ip, 42069);
        await _socket.ConnectAsync(ipEndPoint);
    }

    private async Task EstablishStreamAsync()
    {
        await _sslStream.AuthenticateAsClientAsync("CoryMacdonald");
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
			string messageReceived = await ReadMessageAsync(_sslStream);
            CompareVersion request = JsonSerializer.Deserialize<CompareVersion>(messageReceived) ?? throw new Exception($"Malformed {nameof(CompareVersion)} request from server");

			CompareVersion response = new()
			{
				SemanticVersion = AppInfo.Version
			};

            string strRequest = JsonSerializer.Serialize(response);
			byte[] message = Encoding.UTF8.GetBytes(strRequest + "<EOF>");
			await _sslStream.WriteAsync(message);
			await _sslStream.FlushAsync();

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

    private async Task StartAsync(string ipAddressString = "")
    {
        // TODO: Replace Console.WriteLine() with Serilog stuff and perhaps remove EventService
        if (string.IsNullOrEmpty(ipAddressString))
        {
            ipAddressString = "127.0.0.1";
        }

        IPAddress ip = (await Dns.GetHostEntryAsync(ipAddressString)).AddressList[0] ?? throw new Exception($"Unable to find IP address for {ipAddressString}");
        IPEndPoint ipEndPoint = new(ip, 42069);

        try
        {
            /*// TODO - Crashes when ipStr is "localhost"
            await _clientSocket.ConnectAsync(ipEndPoint);
            Console.WriteLine("Connected!");

            NetworkStream networkStream = new(_clientSocket);
            SslStream sslStream = new(
                networkStream,
                false,
                ValidateServerCertificate,
                null
            );

            await sslStream.AuthenticateAsClientAsync("Cory Macdonald");

            while (true)
            {
                int choice = int.Parse(Console.ReadLine() ?? "0");
                if (choice == 1)
                {
                    int value = int.Parse(Console.ReadLine() ?? "0");

                    if (value > 0)
                    {
                        Create transaction = new()
                        {
                            Value = new RequestField<long>
                            {
                                Value = value
                            },
                            Counterparty = new RequestField<Counterparty>
                            {
                                Value = new Counterparty
                                {
                                    Name = "John"
                                }
                            }
                        };

                        string json = JsonConvert.SerializeObject(transaction);

                        byte[] message = Encoding.UTF8.GetBytes(Create.Flag + json + "<EOF>");
                        sslStream.Write(message);
                        sslStream.Flush();

                        string messageReceived = await ReadMessage(sslStream);
                        CreateResponse? response =
                            JsonConvert.DeserializeObject<CreateResponse>(messageReceived);
                        Console.WriteLine(response);
                    }
                }
                else if (choice == 2)
                {
                    Index request = new()
                    {
                        Page = new RequestField<long>
                        {
                            Value = 0
                        }
                    };

                    string json = JsonConvert.SerializeObject(request);

                    byte[] message = Encoding.UTF8.GetBytes(Index.Flag + json + "<EOF>");
                    sslStream.Write(message);
                    sslStream.Flush();

                    string messageReceived = await ReadMessage(sslStream);
                    IndexResponse? response = JsonConvert.DeserializeObject<IndexResponse>(messageReceived);
                    Console.WriteLine(response);
                }
            }

            _clientSocket.Disconnect();
            Console.WriteLine("Client closed.");*/
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<string> ReadMessageAsync(Stream stream)
    {
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new();
        int bytes;
        do
        {
            bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            messageData.Append(chars);
            if (messageData.ToString().IndexOf("<EOF>", StringComparison.Ordinal) != -1)
            {
                break;
            }
        } while (bytes != 0);

        return messageData.ToString().Replace("<EOF>", "");
    }
}