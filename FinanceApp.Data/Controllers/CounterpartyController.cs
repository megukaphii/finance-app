using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Controllers;

public class CounterpartyController : IController
{
    public static async Task Index(FinanceAppContext database, Client client)
    {
        List<Counterparty> counterparties = await database.Counterparties.ToListAsync();

        GetCounterpartiesResponse response = new()
        {
            Counterparties = counterparties,
            Success = true
        };

        await client.Send(response);
    }
}