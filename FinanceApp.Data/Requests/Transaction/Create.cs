using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using FinanceApp.Data.Validators;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinanceApp.Data.Requests.Transaction;

public class Create : ISingleTransaction
{
    public static string Flag => "<CreateTransaction>";

    public required RequestField<double> Value { get; init; }
    public required RequestField<Counterparty> Counterparty { get; init; }

	public override string ToString()
    {
        return $"{Flag}: [{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
    }

    public async Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        Console.WriteLine(this);

        if (Counterparty.Value.Id == 0) {
            await database.Counterparties.AddAsync(Counterparty.Value);
        }

        Models.Transaction created = new()
        {
            Value = Value.Value,
            Counterparty = Counterparty.Value
        };
        await database.Transactions.AddAsync(created);
        await database.SaveChangesAsync();

        await SendResponseAsync(client, created);
    }

    private async Task SendResponseAsync(SocketStream client, Models.Transaction transaction)
    {
        CreateResponse response = new() {
            Id = transaction.Id,
            Success = true
        };

        // TODO - Extract the following 7 lines into a service or something?
        string strResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
		await client.Stream.WriteAsync(message);
		await client.Stream.FlushAsync();
	}
}