using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinanceApp.Data.Requests.Transaction;

public class GetPage : IPageNumber
{
    public static string Flag => "<GetPageTransactions>";

    public required RequestField<long> Page { get; init; }


	public override string ToString()
    {
        return $"{Flag}: {nameof(Page)}: {Page}";
    }

    public async Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        List<Models.Transaction> transactions =
            await database.Transactions.Include(transaction => transaction.Counterparty).ToListAsync();
        await SendResponse(client, transactions);
    }

    private async Task SendResponse(SocketStream client, List<Models.Transaction> transactions)
    {
        GetPageResponse response = new()
        {
            Transactions = transactions,
            Success = true,
        };

		string strResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions()
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		});
		byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await client.Stream.WriteAsync(message);
        await client.Stream.FlushAsync();
    }
}