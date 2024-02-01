using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Counterparty;

public class UpdateCounterparty : ISingleCounterparty
{
	public static string Flag => "<UpdateCounterparty>";

	public required RequestField<long> Id { get; init; }
	public required RequestField<string> Name { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Id)}: {Id}, {nameof(Name)}: {Name}]";
}

public class UpdateCounterpartyResponse : IResponse
{
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}";
}