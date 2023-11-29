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

        GetAccountsResponse response = new()
        {
            Accounts = accounts,
            Success = true,
        };

        await client.Send(response);
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

        CreateAccountResponse response = new()
        {
            Id = created.Id,
            Success = true
        };

        await client.Send(response);
    }

    public static async Task SetActiveAccount(SelectAccount request, FinanceAppContext database, Client client)
    {
        client.Session.Account = (await database.Accounts.FindAsync(request.Id.Value))!;
        SelectAccountResponse response = new()
        {
            Success = true
        };

        await client.Send(response);
    }
}