using System.Net.Sockets;

namespace FinanceApp.Data.Utility;

public class SocketStream
{
    public required Socket Socket { get; init; }
    public required Stream Stream { get; set; }

    public string IPAddress => Socket.GetIpStr();
}