using FinanceApp.Abstractions;
using System.Net.Security;
using System.Text;
using FinanceApp.Data.Models;

namespace FinanceApp.Data.Requests;

public class CreateTransaction : IRequest
{
    public required string Type { get; set; }
    public required int Value { get; set; }

    public static string Flag => "<CreateTransaction>";

    public override string ToString()
    {
        return $"{nameof(Type)}: {Type}, {nameof(Value)}: {Value}";
    }

    public async Task Handle(IDatabase database, SslStream sslStream)
    {
        Console.WriteLine(this);

        Transaction transaction = new(database, Value, "TEMP");
        transaction.Save();

        await SendResponse(sslStream, transaction);
    }

    private async Task SendResponse(SslStream sslStream, Transaction transaction)
    {
        CreateTransactionResponse response = new() {
            Id = transaction.ID,
            Success = true
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await sslStream.WriteAsync(message);
        sslStream.Flush();
    }
}