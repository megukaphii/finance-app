using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Controllers;

public class AccountController : IController
{
    public static async Task Index(FinanceAppContext database, Client client)
    {
        List<Account> accounts = await database.Accounts.ToListAsync();

        IResponse response = new GetAccountsResponse
        {
            Accounts = accounts,
            Success = true,
        };

        await response.Send<GetAccountsResponse>(client);
    }

    public static async Task Create(CreateAccount request, FinanceAppContext database, Client client)
    {
        Account created = new()
        {
            Name = request.Name.Value,
            Description = request.Description.Value,
            Value = 0
        };
        await database.Accounts.AddAsync(created);
        await database.SaveChangesAsync();

        client.SetActiveAccount(created);

        IResponse response = new CreateAccountResponse
        {
            Id = created.Id,
            Success = true
        };

        await response.Send<CreateAccountResponse>(client);
    }

    public static async Task SetActiveAccount(SelectAccount request, FinanceAppContext database, Client client)
    {
        client.SetActiveAccount((await database.Accounts.FindAsync(request.Id.Value))!);
        IResponse response = new SelectAccountResponse
        {
            Success = true
        };
        await response.Send<SelectAccountResponse>(client);
    }
}