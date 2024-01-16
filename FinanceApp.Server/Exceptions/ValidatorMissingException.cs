namespace FinanceApp.Server.Exceptions;

public class ValidatorMissingException : Exception
{
	public ValidatorMissingException(string? message) : base(message) { }

	public ValidatorMissingException(string? message, Exception? innerException) : base(message, innerException) { }
}