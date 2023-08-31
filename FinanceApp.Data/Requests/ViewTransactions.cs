using FinanceApp.Abstractions;
using System.Text;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.OldModels;

namespace FinanceApp.Data.Requests;

public class ViewTransactions : IRequest {
	public static string Flag => "<ViewTransactions>";

	public int Page { get; init; }

    public override string ToString()
    {
        return $"{Flag}: {nameof(Page)}: {Page}";
    }

    public async Task Handle(IDatabase database, Stream sslStream)
    {
        Console.WriteLine(this);

        EloquentRepository<Transaction> repo = new(database);
        List<Transaction> transactions = repo.All();
        List<SendableTransaction> result = transactions.Select(transaction => new SendableTransaction() { ID = transaction.ID, Value = transaction.Value, Transactee = transaction.Transactee }).ToList();

        await SendResponse(sslStream, result);
    }

    private async Task SendResponse(Stream sslStream, List<SendableTransaction> transactions)
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

    public Task Handle(FinanceAppContext database)
    {
        throw new NotImplementedException();
    }
}