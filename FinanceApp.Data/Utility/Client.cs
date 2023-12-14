using System.Net.Sockets;
using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public class Client : IClient
{
	private readonly Guid _id = Guid.NewGuid();
	private string Id => _id.ToString();
	public required Socket Socket { get; init; }
	public required Stream Stream { get; set; }
	public Session Session { get; } = new();

	public Task<string> ReadMessageAsync() => Stream.ReadMessageAsync();

	public Task Send(IResponse response)
	{
		return Stream.SendResponseAsync(response);
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