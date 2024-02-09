using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Counterparty;

namespace FinanceApp.Data.Requests.Counterparty;

public class CreateCounterparty : ICounterpartyFields
{
	public static string Flag => "<CreateCounterparty>";

	public required RequestField<string> Name { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Name)}: {Name}]";
}

public class CreateCounterpartyResponse : IResponse
{
	public required long Id { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
}