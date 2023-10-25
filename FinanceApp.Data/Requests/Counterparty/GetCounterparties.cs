using FinanceApp.Data.Controllers;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Counterparty;

public class GetCounterparties : IPageNumber
{
    public static string Flag => "<GetCounterparties>";

    public required RequestField<long> Page { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Page)}: {Page}]";
    }

    public Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        return CounterpartyController.Index(database, client);
    }
}