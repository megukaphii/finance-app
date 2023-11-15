using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;
using FinanceApp.Data.Controllers;

namespace FinanceApp.Data.Requests.Transaction;

public class CreateTransaction : ISingleTransaction
{
    public static string Flag => "<CreateTransaction>";

    public required RequestField<decimal> Value { get; init; }
    public required RequestField<Models.Counterparty> Counterparty { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
    }

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return TransactionController.Create(this, database, client);
    }
}