using FinanceApp.Data.Controllers;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Account;

public class CreateAccount : ISingleAccount
{
    public static string Flag => "<CreateAccount>";

    public required RequestField<string> Name { get; init; }
    public required RequestField<string> Description { get; init; }

    public override string ToString()
    {
        return $"{Flag}[{nameof(Name)}: {Name}, {nameof(Description)}: {Description}]";
    }

    public Task HandleAsync(FinanceAppContext database, Client client)
    {
        return AccountController.Create(this, database, client);
    }
}

public class CreateAccountResponse : IResponse
{
    public required bool Success { get; init; }
    public required long Id { get; init; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
    }
}