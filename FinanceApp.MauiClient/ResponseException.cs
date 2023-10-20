using FinanceApp.Data.Interfaces;

namespace FinanceApp.MauiClient;

public class ResponseException<T> : Exception where T : IRequest
{
    public required T Response;
}