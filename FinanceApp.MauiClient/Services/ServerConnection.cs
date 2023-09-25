using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests.Transaction;
using Newtonsoft.Json;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FinanceApp.MauiClient.Services;
public class ServerConnection
{
    public bool IsConnected => _socket.Connected;

    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private SslStream _sslStream;

    public async Task EstablishConnection(string ipAddressStr = "")
    {
        if (string.IsNullOrEmpty(ipAddressStr)) ipAddressStr = "127.0.0.1";

        await ConnectToIP(ipAddressStr);
        await EstablishStream();
	}

    public async Task<CreateResponse> SendButWithStrongCoupling(Create request)
    {
		string json = JsonConvert.SerializeObject(request);
		byte[] message = Encoding.UTF8.GetBytes(Create.Flag + json + "<EOF>");

		_sslStream.Write(message);
		_sslStream.Flush();

		string messageReceived = await ReadMessage(_sslStream);
        return JsonConvert.DeserializeObject<CreateResponse>(messageReceived);
	}

    public async Task<IRequest> SendMessage(IRequest request, Func<string, IRequest> callback)
	{
		string json = JsonConvert.SerializeObject(request);
		byte[] message = Encoding.UTF8.GetBytes(Create.Flag + json + "<EOF>");

		_sslStream.Write(message);
		_sslStream.Flush();

		string messageReceived = await ReadMessage(_sslStream);
        return callback(messageReceived);
	}

    public void Disconnect()
    {
        _socket.Close();
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

    private async Task ConnectToIP(string ipAddressStr)
    {
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddressStr);
        IPAddress ip = hostEntry.AddressList[0] ?? throw new Exception($"Unable to find IP address for {ipAddressStr}");
        IPEndPoint ipEndPoint = new(ip, 42069);
        await _socket.ConnectAsync(ipEndPoint);
    }

    private async Task EstablishStream()
    {
        NetworkStream networkStream = new(_socket);
        _sslStream = new SslStream(networkStream, false, ValidateServerCertificate, null);
        await _sslStream.AuthenticateAsClientAsync("Cory Macdonald");
    }

    private static bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
    {
        /*if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        // Do not allow this client to communicate with unauthenticated servers.
        return false;*/
        return true;
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

            _clientSocket.Close();
            Console.WriteLine("Client closed.");*/
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<string> ReadMessage(Stream sslStream)
    {
        byte[] buffer = new byte[2048];
        StringBuilder messageData = new();
        int bytes;
        do
        {
            bytes = await sslStream.ReadAsync(buffer, 0, buffer.Length);

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