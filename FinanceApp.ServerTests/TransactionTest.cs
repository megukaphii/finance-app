using System.Text;
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

    [SetUp]
    public async Task ClearDB()
    {
        _db.ChangeTracker.Clear();
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Transactions WHERE Id != 0");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='Transactions';");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Counterparties WHERE Id != 0");
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='Counterparties';");
    }

    [Test]
    public async Task CreateTransactionHandle()
    {
        IRequest request = IRequest.GetRequest(Message);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        await request.Handle(_db, stream);

        Transaction? result = await _db.Transactions
            .Include(transaction => transaction.Counterparty)
            .FirstOrDefaultAsync(transaction => transaction.Id == 1);

        Assert.GreaterOrEqual(result?.Id, 1);
        Assert.AreEqual(TestTransaction, result);
    }

    [Test]
    public async Task CreateTransactionResponse()
    {
        IRequest request = IRequest.GetRequest(Message);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        await request.Handle(_db, stream);


        CreateTransactionResponse expected = new()
        {
            Id = 1,
            Success = true
        };
        string message = Encoding.UTF8.GetString(stream.ToArray()).Replace("<EOF>", "");
        CreateTransactionResponse? result = JsonConvert.DeserializeObject<CreateTransactionResponse>(message);

        Assert.AreEqual(expected, result);
    }
}