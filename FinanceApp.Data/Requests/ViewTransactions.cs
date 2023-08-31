using System.Text;
using FinanceApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Requests;

public class ViewTransactions : IRequest {
	public static string Flag => "<ViewTransactions>";

	public int Page { get; init; }

    public override string ToString()
    {
        return $"{Flag}: {nameof(Page)}: {Page}";
    }

    private async Task SendResponse(Stream sslStream, List<Models.Transaction> transactions)
    {
        ViewTransactionResponse response = new() {
            Transactions = transactions,
            Success = true,
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await sslStream.WriteAsync(message);
        await sslStream.FlushAsync();
    }

    public async Task Handle(FinanceAppContext database, Stream stream)
    {
        List<Models.Transaction> transactions = await database.Transactions.ToListAsync();
        await SendResponse(stream, transactions);
    }
}