using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Account;

namespace FinanceApp.Data.Requests.Account;

public class CreateAccount : IAccountFields
{
	public static string Flag => "<CreateAccount>";

	public required RequestField<string> Name { get; init; }
	public required RequestField<string> Description { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Name)}: {Name}, {nameof(Description)}: {Description}]";
}

public class CreateAccountResponse : IResponse
{
	public required long Id { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
}