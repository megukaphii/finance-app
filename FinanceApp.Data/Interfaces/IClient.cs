using System.Net.Sockets;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Interfaces;

public interface IClient
{
	Socket Socket { get; }
	Stream Stream { get; }
	Session Session { get; }
	Task<string> ReadMessageAsync();
	Task Send(IResponse response);
	void WriteLine(object? value);
	void WriteLine(string? value);
}