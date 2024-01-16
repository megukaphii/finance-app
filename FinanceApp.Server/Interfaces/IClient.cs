using System.Net.Sockets;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IClient
{
	Socket Socket { get; }
	Stream Stream { get; }
	ISession Session { get; }
	Task<string> ReadMessageAsync();
	Task Send<T>(T response) where T : IResponse;
	void WriteLine(object? value);
	void WriteLine(string? value);
}