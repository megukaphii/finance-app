using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Exceptions;

public class ResponseException<T> : Exception where T : IRequest
{
    public required T Response;
}