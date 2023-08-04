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
	const long ExodiaTheForbiddenOne = 120;
	const long OtherYugiohReference = 150;
	const string JohnDoe = "John Doe";

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
        Transaction test = new(_db, ExodiaTheForbiddenOne, JohnDoe);

		Assert.AreEqual(0, test.Id);
		test.Save();

		Assert.AreEqual(2, test.Id);
	}

	[Test]
	public void TestSelectTransaction()
    {
		EloquentRepository<Transaction> repo = new(_db);
        Transaction expected = new(_db, ExodiaTheForbiddenOne, JohnDoe) { Id = 1 };

        Transaction? result = repo.Find(1);

        Assert.AreEqual(expected, result);
	}

	[Test]
	public void TestSaveTransaction()
	{
		EloquentRepository<Transaction> repo = new(_db);
		Transaction transaction = repo.Find(1)!;

		transaction.Value = OtherYugiohReference;
		transaction.Save();
		Transaction result = repo.Find(1)!;

		Assert.AreEqual(OtherYugiohReference, result.Value);
	}

    [Test]
    public void TestUpdateTransactionID()
	{
		EloquentRepository<Transaction> repo = new(_db);
		Transaction transaction = repo.Find(1)!;

        Assert.Throws<Exception>(() => transaction.Id = 2);
    }

	[Ignore("Not working on this feature yet")]
	[Test]
	public void TestUpdateTransaction()
	{
		EloquentRepository<Transaction> repo = new(_db);

		//Transaction.Update('value', EXODIA_THE_FORBIDDEN_ONE).Where('value', OTHER_YUGIOH_REFERENCE);
		Transaction transaction = repo.Find(1)!;

		Assert.AreEqual(ExodiaTheForbiddenOne, transaction.Value);
	}

	private void SeedTestData()
	{
		Transaction test = new(_db, ExodiaTheForbiddenOne, JohnDoe);
        test.Save();
	}
}