using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Account;

namespace FinanceApp.Data.Requests.Account;

public class SelectAccount : IAccountId
{
	public static string Flag => "<SelectAccount>";

	public required RequestField<long> Id { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Id)}: {Id}]";
}

public class SelectAccountResponse : IResponse
{
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}";
}