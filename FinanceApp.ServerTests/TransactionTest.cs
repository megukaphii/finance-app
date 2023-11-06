using System.Net.Sockets;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using NUnit.Framework;
using GetPage = FinanceApp.Data.Requests.Transaction.GetPage;

namespace FinanceApp.ServerTests;

[TestFixture]
public class TransactionTest
{
	private static readonly FinanceAppContext _db = new();
	private static readonly Socket EmptySocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private static readonly Transaction TestTransaction = new()
    {
        Account = new()
        {
            Name = "Test Acc.",
            Description = "Test Desc.",
            Value = 0
        },
        Counterparty = new()
        {
            Name = "John"
        },
        Value = 100
    };

    private static readonly Create TestCreateRequest = new()
    {
        Value = new()
        {
            Value = TestTransaction.Value
        },
        Counterparty = new()
        {
            Value = TestTransaction.Counterparty
        }
    };

	private static readonly string MessageCreteRequest = $"{Create.Flag}{JsonSerializer.Serialize(TestCreateRequest)}";

    private static readonly GetPage TestIndexRequest = new()
    {
        Page = new()
        {
            Value = 0
        }
    };

	private static readonly string MessageIndexRequest = $"{GetPage.Flag}{JsonSerializer.Serialize(TestIndexRequest)}";

    private static readonly Counterparty SeedCounterparty1 = new() { Name = "John Doe" };
    private static readonly Counterparty SeedCounterparty2 = new() { Name = "Megumin" };
    private static readonly List<Transaction> SeedTransactions = new()
    {
        new()
        {
            Account = new()
            {
                Name = "Test Acc.",
                Description = "Test Desc.",
                Value = 0
            },
            Counterparty = SeedCounterparty1,
            Value = 10
        },
        new()
        {
            Account = new()
            {
                Name = "Test Acc.",
                Description = "Test Desc.",
                Value = 0
            },
            Counterparty = SeedCounterparty1,
            Value = -50
        },
        new()
        {
            Account = new()
            {
                Name = "Test Acc.",
                Description = "Test Desc.",
                Value = 0
            },
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
        SocketStream client = new SocketStream() { Socket = EmptySocket, Stream =  stream };
        await request.HandleAsync(_db, client);

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
		SocketStream client = new() { Socket = EmptySocket, Stream = stream };
		await request.HandleAsync(_db, client);

		CreateResponse expected = new()
        {
            Id = 1,
            Success = true
        };
        string message = Encoding.UTF8.GetString(stream.ToArray());
		message = Helpers.RemoveFromEof(message);
		CreateResponse? result = JsonSerializer.Deserialize<CreateResponse>(message);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public async Task TransactionIndex()
    {
        await SeedDB();

        IRequest request = IRequest.GetRequest(MessageIndexRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
		SocketStream client = new() { Socket = EmptySocket, Stream = stream };
		await request.HandleAsync(_db, client);

		string message = Encoding.UTF8.GetString(stream.ToArray());
        message = Helpers.RemoveFromEof(message);
		GetPageResponse? result = JsonSerializer.Deserialize<GetPageResponse>(message);

        Assert.True(result?.Success);
        Assert.AreEqual(SeedTransactions, result?.Transactions);
    }

    private async Task SeedDB()
    {
        await _db.Transactions.AddRangeAsync(SeedTransactions);
        await _db.SaveChangesAsync();
    }
}