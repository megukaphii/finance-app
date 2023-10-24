﻿using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using FinanceApp.Data.Controllers;

namespace FinanceApp.Data.Requests.Transaction;

public class Create : ISingleTransaction
{
    public static string Flag => "<CreateTransaction>";

    public required RequestField<double> Value { get; init; }
    public required RequestField<Counterparty> Counterparty { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
    }

    public Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        return TransactionController.Create(this, database, client);
    }
}