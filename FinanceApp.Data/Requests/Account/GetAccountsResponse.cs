using System.Text;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Account;

public class GetAccountsResponse : IResponse
{
	public required bool Success { get; init; }
	public required List<Models.Account> Accounts { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Accounts.Count}, [{nameof(Accounts)}: {string.Join(", ", Accounts)}]");
        return result.ToString();
    }
}