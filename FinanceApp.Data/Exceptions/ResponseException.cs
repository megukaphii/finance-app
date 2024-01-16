using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Exceptions;

public class ResponseException<T> : Exception where T : IRequest
{
	public required T Response;

	public ResponseException() { }

	public ResponseException(string? message) : base(message) { }

	public ResponseException(string? message, Exception? innerException) : base(message, innerException) { }
}