using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Transaction;

public class GetTransactions : IPageNumber
{
	public static string Flag => "<GetTransactions>";

	public required RequestField<long> Page { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Page)}: {Page}]";
}

public class GetTransactionsResponse : IResponse
{
	public required List<Models.Transaction> Transactions { get; init; }
	public required bool Success { get; init; }

	public override string ToString() =>
		$"{nameof(Success)}: {Success}, Count: {Transactions.Count}, [{nameof(Transactions)}: {string.Join(", ", Transactions)}]";
}