using System.Net.Sockets;
using System.Text;
using FinanceApp.Data;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
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
	private static readonly FinanceAppContext DB = new();
	private static readonly Socket EmptySocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private static readonly Transaction TestTransaction = new()
    {
        Counterparty = new()
        {
            Name = "John"
        },
        Value = 100,
        Timestamp = DateTime.Now.Date
    };

    private static readonly Create TestCreateRequest = new()
    {
        Counterparty = new()
        {
            Value = TestTransaction.Counterparty
        },
        Value = new()
        {
            Value = TestTransaction.Value
        },
        Timestamp = new()
        {
            Value = TestTransaction.Timestamp
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
            Counterparty = SeedCounterparty1,
            Value = 10,
            Timestamp = DateTime.Now.Date
        },
        new()
        {
            Counterparty = SeedCounterparty1,
            Value = -50,
            Timestamp = DateTime.Now.Date
        },
        new()
        {
            Counterparty = SeedCounterparty2,
            Value = 420,
            Timestamp = DateTime.Now.Date
        }
    };

    [OneTimeSetUp]
    public async Task PerformMigrations()
    {
        if ((await DB.Database.GetPendingMigrationsAsync()).Any()) {
            await DB.Database.MigrateAsync();
        }
    }

    [SetUp]
    public async Task ClearDB()
    {
        DB.ChangeTracker.Clear();
        await DB.Database.ExecuteSqlRawAsync("DELETE FROM Transactions WHERE Id != 0");
        await DB.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='Transactions';");
        await DB.Database.ExecuteSqlRawAsync("DELETE FROM Counterparties WHERE Id != 0");
        await DB.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='Counterparties';");
    }

    [Test]
    public async Task TransactionCreateHandle()
    {
        IRequest request = IRequest.GetRequest(MessageCreteRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        SocketStream client = new SocketStream() { Socket = EmptySocket, Stream =  stream };
        await request.HandleAsync(DB, client);

        Transaction? result = await DB.Transactions
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
		await request.HandleAsync(DB, client);

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
		await request.HandleAsync(DB, client);

		string message = Encoding.UTF8.GetString(stream.ToArray());
        message = Helpers.RemoveFromEof(message);
		GetPageResponse? result = JsonSerializer.Deserialize<GetPageResponse>(message);

        Assert.True(result?.Success);
        Assert.AreEqual(SeedTransactions, result?.Transactions);
    }

    private async Task SeedDB()
    {
        await DB.Transactions.AddRangeAsync(SeedTransactions);
        await DB.SaveChangesAsync();
    }
}