using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Controllers;

public class TransactionController : IController
{
    public static async Task Index(FinanceAppContext database, Client client)
    {
        List<Transaction> transactions =
            await database.Transactions
                .Where(transaction => transaction.Account.Equals(client.Session.Account))
                .Include(transaction => transaction.Counterparty)
                .ToListAsync();

        GetTransactionsResponse response = new()
        {
            Transactions = transactions,
            Success = true,
        };

        await client.Send(response);
    }

    public static async Task Create(CreateTransaction request, FinanceAppContext database, Client client)
    {
        Counterparty? counterparty = request.Counterparty.Value;
        if (request.Counterparty.Value.Id == 0) {
            counterparty = await database.Counterparties.FirstOrDefaultAsync(temp => temp.Name == request.Counterparty.Value.Name);
            if (counterparty is null) {
                await database.Counterparties.AddAsync(request.Counterparty.Value);
                counterparty = request.Counterparty.Value;
            }
        }

        if (!client.Session.IsAccountSet()) {
            // TODO - Make sure this is handled properly! Probably doesn't currently work
            CreateAccountResponse response = new()
            {
                Id = 0,
                Success = false
            };

            await client.Send(response);
        } else {
            Transaction created = new()
            {
                Account = client.Session.Account,
                Counterparty = counterparty,
                Value = request.Value.Value
            };
            await database.Transactions.AddAsync(created);
            await database.SaveChangesAsync();

            CreateAccountResponse response = new()
            {
                Id = created.Id,
                Success = true
            };

            await client.Send(response);
        }
    }
}