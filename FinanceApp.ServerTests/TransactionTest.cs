using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Data;
using FinanceApp.Server.Services;

namespace FinanceApp.ServerTests;

[TestClass]
public class TransactionTest {
	const long EXODIA_THE_FORBIDDEN_ONE = 120;
	const long OTHER_YUGIOH_REFERENCE = 150;
	const string JOHN_DOE = "John Doe";

	[TestInitialize()]
	public void TestInit() {
		MigrationService ms = new();
		ms.RefreshTables<SqliteDatabase>();
		SeedTestData();
	}

	[TestMethod]
	public void TestCreateTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction test = new(db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE);

			Assert.AreEqual(0, test.ID);

			test.Save();

			Assert.AreEqual(2, test.ID);
		}
	}

	[TestMethod]
	public void TestSelectTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction expected = new(db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE);
			expected.ID = 1;

			Transaction? result = Transaction.Find(1);

			Assert.AreEqual(expected, result);
		}
	}

	[TestMethod]
	public void TestSaveTransaction() {
		using (SqliteDatabase db = new()) {
			// [TODO] Find never fills Database property, so transaction cannot properly interact with DB
			Transaction transaction = Transaction.Find(1)!;
			transaction.Value = OTHER_YUGIOH_REFERENCE;
			transaction.Save();

			Transaction result = Transaction.Find(1)!;

			Assert.AreEqual(OTHER_YUGIOH_REFERENCE, result.Value);
		}
	}

    [TestMethod]
    public void TestUpdateTransactionID() {
        using (SqliteDatabase db = new()) {
            Transaction transaction = Transaction.Find(1)!;
            Assert.ThrowsException<Exception>(() => transaction.ID = 2);
        }
    }

	[Ignore]
	[TestMethod]
	public void TestUpdateTransaction() {
		using (SqliteDatabase db = new()) {
			//Transaction.Update('value', EXODIA_THE_FORBIDDEN_ONE).Where('value', OTHER_YUGIOH_REFERENCE);

			Transaction transaction = Transaction.Find(1)!;

			Assert.AreEqual(EXODIA_THE_FORBIDDEN_ONE, transaction.Value);
		}
	}

	private void SeedTestData() {
		using (SqliteDatabase db = new()) {
			Transaction test = new(db, EXODIA_THE_FORBIDDEN_ONE, JOHN_DOE);
			test.Save();
		}
	}
}