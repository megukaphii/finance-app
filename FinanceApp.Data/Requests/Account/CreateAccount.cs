using FinanceApp.Data.Controllers;
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