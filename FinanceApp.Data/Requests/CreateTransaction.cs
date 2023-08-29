﻿using FinanceApp.Abstractions;
using System.Text;
using FinanceApp.Data.OldModels;

namespace FinanceApp.Data.Requests;

public class CreateTransaction : IRequest
{
    public static string Flag => "<CreateTransaction>";

    public required int Value { get; init; }

    public override string ToString()
    {
        return $"{Flag}: {nameof(Value)}: {Value}";
    }

    public async Task Handle(IDatabase database, Stream sslStream)
    {
        Console.WriteLine(this);

        Transaction transaction = new(database, Value, "TEMP");
        transaction.Save();

        await SendResponse(sslStream, transaction);
    }

    private async Task SendResponse(Stream sslStream, Transaction transaction)
    {
        CreateTransactionResponse response = new() {
            Id = transaction.ID,
            Success = true
        };
        string strResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        byte[] message = Encoding.UTF8.GetBytes(strResponse + "<EOF>");
        await sslStream.WriteAsync(message);
        await sslStream.FlushAsync();
    }
}