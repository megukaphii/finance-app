using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns.Transaction;

public interface ITransactionFields : IRequest
{
	public RequestField<decimal> Value { get; init; }
	public RequestField<long> Counterparty { get; init; }
    public RequestField<DateTime> Timestamp { get; init; }
}