using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns.Counterparty;

public interface ICounterpartyFields : IRequest
{
	public RequestField<string> Name { get; init; }
}