using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests;
using Newtonsoft.Json;
using Services.Interfaces;

namespace FinanceApp.MauiClient.Services.Implementations
{
    class ServerService : IServerService
    {
        private readonly Socket _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly IEventService _eventService;

        public ServerService(IEventService eventService)
        {
            _eventService = eventService;
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

        public async Task StartAsync(string ipAddressString = "")
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
                // TODO - Crashes when ipStr is "localhost"
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
                            CreateTransaction transaction = new()
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

                            byte[] message = Encoding.UTF8.GetBytes(CreateTransaction.Flag + json + "<EOF>");
                            sslStream.Write(message);
                            sslStream.Flush();

                            string messageReceived = await ReadMessage(sslStream);
                            CreateTransactionResponse? response =
                                JsonConvert.DeserializeObject<CreateTransactionResponse>(messageReceived);
                            Console.WriteLine(response);
                        }
                    }
                    else if (choice == 2)
                    {
                        ViewTransactions request = new()
                        {
                            Page = 0
                        };

                        string json = JsonConvert.SerializeObject(request);

                        byte[] message = Encoding.UTF8.GetBytes(ViewTransactions.Flag + json + "<EOF>");
                        sslStream.Write(message);
                        sslStream.Flush();

                        string messageReceived = await ReadMessage(sslStream);
                        ViewTransactionResponse? response = JsonConvert.DeserializeObject<ViewTransactionResponse>(messageReceived);
                        Console.WriteLine(response);
                    }
                }

                _clientSocket.Close();
                Console.WriteLine("Client closed.");
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
}