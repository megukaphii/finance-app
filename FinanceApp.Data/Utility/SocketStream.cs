using System.Net.Sockets;

namespace FinanceApp.Data.Utility;

public class SocketStream
{
    private readonly Guid _id = new();
    public required Socket Socket { get; init; }
    public required Stream Stream { get; set; }

    public string Id => _id.ToString();
}