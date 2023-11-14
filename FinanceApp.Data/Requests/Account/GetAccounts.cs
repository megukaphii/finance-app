using FinanceApp.Data.Controllers;
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