using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using FinanceApp.Data.Controllers;

namespace FinanceApp.Data.Requests.Transaction;

public class GetPage : IPageNumber
{
    public static string Flag => "<GetPageTransactions>";

    public required RequestField<long> Page { get; init; }

	public override string ToString()
    {
        return $"{Flag}: {nameof(Page)}: {Page}";
    }

    public Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        return TransactionController.Index(database, client);
    }
}