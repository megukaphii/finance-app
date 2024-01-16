using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Counterparty;

public class GetCounterparties : IPageNumber
{
	public static string Flag => "<GetCounterparties>";

	public required RequestField<long> Page { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Page)}: {Page}]";
}

public class GetCounterpartiesResponse : IResponse
{
	public required List<Models.Counterparty> Counterparties { get; init; }
	public required bool Success { get; init; }

	public override string ToString() =>
		$"{nameof(Success)}: {Success}, Count: {Counterparties.Count}, [{nameof(Counterparties)}: {string.Join(", ", Counterparties)}]";
}