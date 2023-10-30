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
        Counterparty? counterparty = request.Counterparty.Value;
        if (request.Counterparty.Value.Id == 0) {
            counterparty = await database.Counterparties.FirstOrDefaultAsync(temp => temp.Name == request.Counterparty.Value.Name);
            if (counterparty is null) {
                await database.Counterparties.AddAsync(request.Counterparty.Value);
                counterparty = request.Counterparty.Value;
            }
        }

        Transaction created = new()
        {
            Value = request.Value.Value,
            Counterparty = counterparty
        };
        await database.Transactions.AddAsync(created);
        await database.SaveChangesAsync();

        IResponse response = new CreateResponse
        {
            Id = created.Id,
            Success = true
        };

        await response.Send<CreateResponse>(client);
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

        await response.Send<GetPageResponse>(client);
    }
}