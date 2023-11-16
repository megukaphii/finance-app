using FinanceApp.Data.Controllers;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Account;

public class SelectAccount : IAccountId
{
    public static string Flag => "<SelectAccount>";

    public required RequestField<long> Id { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Id)}: {Id}]";
    }

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return AccountController.SetActiveAccount(this, database, client);
    }
}

public class SelectAccountResponse : IResponse
{
    public required bool Success { get; init; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}";
    }
}