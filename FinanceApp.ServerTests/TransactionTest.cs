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

namespace FinanceApp.ServerTests;

[TestFixture]
public class TransactionTest
{
	private static readonly FinanceAppContext DB = new();
	private static readonly Socket EmptySocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private static readonly Account DefaultAccount = new()
    {
        Name = "Test Acc.",
        Description = "Test Desc.",
        Value = 0
    };

    private static readonly Transaction TestTransaction = new()
    {
        Account = DefaultAccount,
        Counterparty = new()
        {
            Name = "John"
        },
        Value = 100
    };

    private static readonly CreateTransaction TestCreateTransactionRequest = new()
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

	private static readonly string MessageCreteRequest = $"{CreateTransaction.Flag}{JsonSerializer.Serialize(TestCreateTransactionRequest)}";

    private static readonly GetTransactions TestIndexRequest = new()
    {
        Page = new()
        {
            Value = 0
        }
    };

	private static readonly string MessageIndexRequest = $"{GetTransactions.Flag}{JsonSerializer.Serialize(TestIndexRequest)}";

    private static readonly Counterparty SeedCounterparty1 = new() { Name = "John Doe" };
    private static readonly Counterparty SeedCounterparty2 = new() { Name = "Megumin" };
    private static readonly List<Transaction> SeedTransactions = new()
    {
        new()
        {
            Account = DefaultAccount,
            Counterparty = SeedCounterparty1,
            Value = 10
        },
        new()
        {
            Account = DefaultAccount,
            Counterparty = SeedCounterparty1,
            Value = -50
        },
        new()
        {
            Account = DefaultAccount,
            Counterparty = SeedCounterparty2,
            Value = 420
        }
    };

    [OneTimeSetUp]
    public async Task PerformMigrations()
    {
        if ((await DB.Database.GetPendingMigrationsAsync()).Any()) {
            await DB.Database.MigrateAsync();
        }

        if (!await DB.Accounts.AnyAsync()) {
            await DB.Accounts.AddAsync(DefaultAccount);
            await DB.SaveChangesAsync();
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

    // TODO - These need a lot of reworking, I think. First do the TODO in FinanceAppContext though.
    [Test]
    public async Task TransactionCreateHandle()
    {
        IRequest request = IRequest.GetRequest(MessageCreteRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
        Client client = new()
        {
            Socket = EmptySocket, Stream = stream,
            Session =
            {
                Account = await DB.Accounts.FirstAsync()
            }
        };
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
		Client client = new()
        {
            Socket = EmptySocket, Stream = stream,
            Session =
            {
                Account = await DB.Accounts.FirstAsync()
            }
        };
        await request.HandleAsync(DB, client);

		CreateTransactionResponse expected = new()
        {
            Id = 1,
            Success = true
        };
        string message = Encoding.UTF8.GetString(stream.ToArray());
		message = Helpers.RemoveFromEof(message);
		CreateTransactionResponse? result = JsonSerializer.Deserialize<CreateTransactionResponse>(message);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public async Task TransactionIndex()
    {
        await SeedDB();

        IRequest request = IRequest.GetRequest(MessageIndexRequest);
        byte[] buffer = new byte[2048];
        MemoryStream stream = new(buffer);
		Client client = new()
        {
            Socket = EmptySocket, Stream = stream,
            Session =
            {
                Account = await DB.Accounts.FirstAsync()
            }
        };
        await request.HandleAsync(DB, client);

		string message = Encoding.UTF8.GetString(stream.ToArray());
        message = Helpers.RemoveFromEof(message);
		GetTransactionsResponse? result = JsonSerializer.Deserialize<GetTransactionsResponse>(message);

        Assert.True(result?.Success);
        Assert.AreEqual(SeedTransactions, result?.Transactions);
    }

    private static async Task SeedDB()
    {
        Account tempAccount = await DB.Accounts.FirstAsync();
        List<Transaction> tempTransactions = SeedTransactions.Select(
            template => new Transaction
            {
                Account = tempAccount,
                Counterparty = template.Counterparty,
                Value = template.Value
            }
        ).ToList();

        await DB.Transactions.AddRangeAsync(tempTransactions);
        await DB.SaveChangesAsync();
    }
}