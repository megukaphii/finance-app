using FinanceApp.Abstractions;
using System.Text;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests;

public class CreateTransaction : ISingleTransaction
{
    public static string Flag => "<CreateTransaction>";

    public required int Value { get; init; }
    public required Counterparty Counterparty { get; init; }

    public override string ToString()
    {
        return $"{Flag}: {nameof(Value)}: {Value}";
    }

    public async Task Handle(FinanceAppContext database)
    {
        Console.WriteLine(this);

        if (Counterparty.Id == 0) {
            await database.Counterparties.AddAsync(Counterparty);
        }

        Transaction created = new()
        {
            Value = Value,
            Counterparty = Counterparty
        };
        await database.Transactions.AddAsync(created);
        await database.SaveChangesAsync();

        //await SendResponse(sslStream, created);
    }

    private async Task SendResponse(Stream sslStream, Transaction transaction)
    {
        CreateTransactionResponse response = new() {
            Id = transaction.Id,
            Success = true
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await sslStream.WriteAsync(message);
        await sslStream.FlushAsync();
    }
}