using System.Text;
using FinanceApp.Data.Controllers;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Account;

public class GetAccounts : IPageNumber
{
    public static string Flag => "<GetAccounts>";

    public required RequestField<long> Page { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Page)}: {Page}]";
    }

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return AccountController.Index(database, client);
    }
}

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