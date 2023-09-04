using System.Text;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinanceApp.Data.Requests;

public class TransactionIndex : IPageNumber
{
    public static string Flag => "<ViewTransactions>";

    public required RequestField<long> Page { get; init; }

    public override string ToString()
    {
        return $"{Flag}: {nameof(Page)}: {Page}";
    }

    public async Task Handle(FinanceAppContext database, Stream stream)
    {
        List<Transaction> transactions =
            await database.Transactions.Include(transaction => transaction.Counterparty).ToListAsync();
        await SendResponse(stream, transactions);
    }

    private async Task SendResponse(Stream stream, List<Transaction> transactions)
    {
        TransactionIndexResponse indexResponse = new()
        {
            Transactions = transactions,
            Success = true,
        };
        string strResponse = JsonConvert.SerializeObject(
            indexResponse,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
        );

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await stream.WriteAsync(message);
        await stream.FlushAsync();
    }
}