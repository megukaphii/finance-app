using System.Text;
using FinanceApp.Data.Controllers;
using FinanceApp.Data.Interfaces;
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

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return CounterpartyController.Index(database, client);
    }
}

public class GetCounterpartiesResponse : IResponse
{
    public required bool Success { get; init; }
    public required List<Models.Counterparty> Counterparties { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Counterparties.Count}, [{nameof(Counterparties)}: {string.Join(", ", Counterparties)}]");
        return result.ToString();
    }
}