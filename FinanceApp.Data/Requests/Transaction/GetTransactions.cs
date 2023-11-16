using System.Text;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using FinanceApp.Data.Controllers;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Transaction;

public class GetTransactions : IPageNumber
{
    public static string Flag => "<GetTransactions>";

    public required RequestField<long> Page { get; init; }

	public override string ToString()
    {
        return $"{Flag}[{nameof(Page)}: {Page}]";
    }

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return TransactionController.Index(database, client);
    }
}

public class GetTransactionsResponse : IResponse
{
    public required bool Success { get; init; }
    public required List<Models.Transaction> Transactions { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Transactions.Count}, [{nameof(Transactions)}: {string.Join(", ", Transactions)}]");
        return result.ToString();
    }
}