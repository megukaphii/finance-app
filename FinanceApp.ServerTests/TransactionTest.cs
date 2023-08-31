using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FinanceApp.ServerTests;

[TestFixture]
public class TransactionTest {
    private readonly FinanceAppContext _db = new();

    private static readonly Transaction TestTransaction = new()
    {
        Value = 100,
        Counterparty = new Counterparty
        {
            Name = "John"
        }
    };
    private static readonly CreateTransaction TestRequest = new()
    {
        Value = TestTransaction.Value,
        Counterparty = TestTransaction.Counterparty
    };
    private static readonly string Message = $"<CreateTransaction>{JsonConvert.SerializeObject(TestRequest)}";

    [Test]
    public async Task TestCreateTransaction()
    {
        IRequest request = IRequest.GetRequest(Message);
        await request.Handle(_db);

        Transaction? result = await _db.Transactions
            .Include(transaction => transaction.Counterparty)
            .FirstOrDefaultAsync(transaction => transaction.Id == 1);
        TestTransaction.Id = result?.Id ?? 0;
        TestTransaction.Counterparty.Id = result?.Counterparty.Id ?? 0;

        Assert.AreEqual(TestTransaction, result);
    }
}