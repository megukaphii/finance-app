using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using FinanceApp.Data.Extensions;
using FinanceApp.Data.Models;

namespace FinanceApp.Data.Utility;

public class Client
{
    private readonly Guid _id = Guid.NewGuid();
    private string Id => _id.ToString();
    public required Socket Socket { get; init; }
    public required Stream Stream { get; set; }
    public Session Session { get; } = new();

    public Task<string> ReadMessageAsync()
    {
        return Stream.ReadMessageAsync();
    }

    public void SetActiveAccount(Account active)
    {
        Session.Account = active;
    }

    public void WriteLine(string? value)
    {
        Console.WriteLine($"[{Id}] {value}");
    }
}