using System.Net.Sockets;
using FinanceApp.Data.Models;

namespace FinanceApp.Data.Utility;

public class Client
{
    private readonly Guid _id = Guid.NewGuid();
    public required Socket Socket { get; init; }
    public required Stream Stream { get; set; }
    public Session Session { get; } = new();

    public string Id => _id.ToString();

    public void SetActiveAccount(Account active)
    {
        Session.Account = active;
    }
}