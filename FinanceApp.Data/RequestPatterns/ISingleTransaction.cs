using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
	public RequestField<decimal> Value { get; init; }
	public RequestField<long> Counterparty { get; init; }
    public RequestField<DateTime> Timestamp { get; init; }
}