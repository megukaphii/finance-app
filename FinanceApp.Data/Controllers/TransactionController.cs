using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Controllers;

public abstract class TransactionController : IController
{
    public static async Task Create(Create request, FinanceAppContext database, SocketStream client)
    {
        if (request.Counterparty.Value.Id == 0) {
            await database.Counterparties.AddAsync(request.Counterparty.Value);
        }

        Transaction created = new()
        {
            Value = request.Value.Value,
            Counterparty = request.Counterparty.Value
        };
        await database.Transactions.AddAsync(created);
        await database.SaveChangesAsync();

        IResponse response = new CreateResponse
        {
            Id = created.Id,
            Success = true
        };

        await response.Send(client);
    }

    public static async Task Index(FinanceAppContext database, SocketStream client)
    {
        List<Transaction> transactions =
            await database.Transactions.Include(transaction => transaction.Counterparty).ToListAsync();

        IResponse response = new GetPageResponse
        {
            Transactions = transactions,
            Success = true,
        };

        await response.Send(client);
    }
}