namespace FinanceApp.Data.RequestPatterns.Counterparty;

public interface ICounterpartyFull : ICounterpartyFields
{
	public RequestField<long> Id { get; init; }
}