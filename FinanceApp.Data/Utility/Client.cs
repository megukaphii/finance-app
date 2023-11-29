using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;

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

    public async Task Send<T>(T response) where T : IResponse
    {
        string strResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
        byte[] message = Encoding.UTF8.GetBytes(strResponse + Serialization.Eof);
        await Stream.WriteAsync(message);
        await Stream.FlushAsync();
    }

    public void WriteLine(object? value)
    {
        Console.WriteLine($"[{Id}] {value}");
    }

    public void WriteLine(string? value)
    {
        Console.WriteLine($"[{Id}] {value}");
    }
}