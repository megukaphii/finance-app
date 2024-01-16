﻿namespace FinanceApp.Server.Exceptions;

public class InvalidMessageException : Exception
{
	public InvalidMessageException(string? message) : base(message) { }

	public InvalidMessageException(string? message, Exception? innerException) : base(message, innerException) { }
}