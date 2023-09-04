using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests;
using FinanceApp.Data.Requests.Transaction;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using Index = FinanceApp.Data.Requests.Transaction.Index;

namespace FinanceApp.ServerTests;

[TestFixture]
public class TransactionTest
{
    private readonly FinanceAppContext _db = new();

    private static readonly Transaction TestTransaction = new()
    {
        Value = 100,
        Counterparty = new Counterparty
        {
            Name = "John"
        }
    };

    private static readonly Create TestCreateRequest = new()
    {
        Value = new RequestField<long>
        {
            Value = TestTransaction.Value
        },
        Counterparty = new RequestField<Counterparty>
        {
            Value = TestTransaction.Counterparty
        }
    };

    private static readonly string MessageCreteRequest = $"{Create.Flag}{JsonConvert.SerializeObject(TestCreateRequest)}";

    private static readonly Index TestIndexRequest = new()
    {
        Page = new RequestField<long>
        {
            Value = 0
        }
    };

    private static readonly string MessageIndexRequest = $"{Index.Flag}{JsonConvert.SerializeObject(TestIndexRequest)}";

    private static readonly Counterparty SeedCounterparty1 = new() { Name = "John Doe" };
    private static readonly Counterparty SeedCounterparty2 = new() { Name = "Megumin" };
    private static readonly List<Transaction> SeedTransactions = new()
    {
        new Transaction
        {
            Counterparty = SeedCounterparty1,
            Value = 10
        },
        new Transaction
        {
            Counterparty = SeedCounterparty1,
            Value = -50
        },
        new Transaction
        {
            Counterparty = SeedCounterparty2,
            Value = 420
        }
    };

    [OneTimeSetUp]
    public async Task PerformMigrations()
    {
        if ((await _db.Database.GetPendingMigrationsAsync()).Any()) {
            await _db.Database.MigrateAsync();
        }
    }

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
    public async Task TransactionCreateHandle()
    {
        IRequest request = IRequest.GetRequest(MessageCreteRequest);
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
    public async Task TransactionCreateResponse()
    {
        IRequest request = IRequest.GetRequest(MessageCreteRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        await request.Handle(_db, stream);

        CreateResponse expected = new()
        {
            Id = 1,
            Success = true
        };
        string message = Encoding.UTF8.GetString(stream.ToArray()).Replace("<EOF>", "");
        CreateResponse? result = JsonConvert.DeserializeObject<CreateResponse>(message);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public async Task TransactionIndex()
    {
        await SeedDB();

        IRequest request = IRequest.GetRequest(MessageIndexRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        await request.Handle(_db, stream);

        string message = Encoding.UTF8.GetString(stream.ToArray()).Replace("<EOF>", "");
        IndexResponse? result = JsonConvert.DeserializeObject<IndexResponse>(message);

        Assert.True(result?.Success);
        Assert.AreEqual(SeedTransactions, result?.Transactions);
    }

    private async Task SeedDB()
    {
        await _db.Transactions.AddRangeAsync(SeedTransactions);
        await _db.SaveChangesAsync();
    }
}