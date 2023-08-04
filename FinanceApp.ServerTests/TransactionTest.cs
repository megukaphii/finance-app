using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Data.Models;
using FinanceApp.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace FinanceApp.ServerTests;

[TestFixture]
public class TransactionTest {
	const long EXODIA_THE_FORBIDDEN_ONE = 120;
	const long OTHER_YUGIOH_REFERENCE = 150;
	const string JOHN_DOE = "John Doe";

#pragma warning disable CS8618
    private IDatabase _db;
#pragma warning restore CS8618

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IDatabase, SqliteDatabase>();
            })
            .Build();

		_db = host.Services.GetRequiredService<IDatabase>();
    }

    [SetUp]
	public void SetUp()
    {
		MigrationService ms = new();
		ms.RefreshTables<SqliteDatabase>();
		SeedTestData();
	}

	[Test]
	public void TestCreateTransaction()
    {
        Transaction test = new(_db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE);

		Assert.AreEqual(0, test.ID);
		test.Save();

		Assert.AreEqual(2, test.ID);
	}

	[Test]
	public void TestSelectTransaction()
    {
		EloquentRepository<Transaction> repo = new(_db);
        Transaction expected = new(_db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE) { ID = 1 };

        Transaction? result = repo.Find(1);

        Assert.AreEqual(expected, result);
	}

	[Test]
	public void TestSaveTransaction()
	{
		EloquentRepository<Transaction> repo = new(_db);
		Transaction transaction = repo.Find(1)!;

		transaction.Value = OTHER_YUGIOH_REFERENCE;
		transaction.Save();
		Transaction result = repo.Find(1)!;

		Assert.AreEqual(OTHER_YUGIOH_REFERENCE, result.Value);
	}

    [Test]
    public void TestUpdateTransactionID()
	{
		EloquentRepository<Transaction> repo = new(_db);
		Transaction transaction = repo.Find(1)!;

        Assert.Throws<Exception>(() => transaction.ID = 2);
    }

	[Ignore("Not working on this feature yet")]
	[Test]
	public void TestUpdateTransaction()
	{
		EloquentRepository<Transaction> repo = new(_db);

		//Transaction.Update('value', EXODIA_THE_FORBIDDEN_ONE).Where('value', OTHER_YUGIOH_REFERENCE);
		Transaction transaction = repo.Find(1)!;

		Assert.AreEqual(EXODIA_THE_FORBIDDEN_ONE, transaction.Value);
	}

	private void SeedTestData()
	{
		Transaction test = new(_db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE);
        test.Save();
	}
}