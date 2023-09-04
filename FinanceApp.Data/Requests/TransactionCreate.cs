using System.Text;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests;

public class TransactionCreate : ISingleTransaction
{
    public static string Flag => "<CreateTransaction>";

    public required RequestField<long> Value { get; init; }
    public required RequestField<Counterparty> Counterparty { get; init; }

    public override string ToString()
    {
        return $"{Flag}: [{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
    }

    public async Task Handle(FinanceAppContext database, Stream stream)
    {
        Console.WriteLine(this);

        if (Counterparty.Value.Id == 0) {
            await database.Counterparties.AddAsync(Counterparty.Value);
        }

        Transaction created = new()
        {
            Value = Value.Value,
            Counterparty = Counterparty.Value
        };
        await database.Transactions.AddAsync(created);
        await database.SaveChangesAsync();

        await SendResponse(stream, created);
    }

    private async Task SendResponse(Stream stream, Transaction transaction)
    {
        TransactionCreateResponse response = new() {
            Id = transaction.Id,
            Success = true
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await stream.WriteAsync(message);
        await stream.FlushAsync();
    }
}