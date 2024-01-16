using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Account;

public class GetAccounts : IPageNumber
{
	public static string Flag => "<GetAccounts>";

	public required RequestField<long> Page { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Page)}: {Page}]";
}

public class GetAccountsResponse : IResponse
{
	public required List<Models.Account> Accounts { get; init; }
	public required bool Success { get; init; }

	public override string ToString() =>
		$"{nameof(Success)}: {Success}, Count: {Accounts.Count}, [{nameof(Accounts)}: {string.Join(", ", Accounts)}]";
}