using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns.Counterparty;

public interface ICounterpartyFull : IRequest
{
	public RequestField<long> Id { get; init; }
	public RequestField<string> Name { get; init; }
}