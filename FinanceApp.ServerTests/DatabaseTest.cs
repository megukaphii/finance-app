using FinanceApp.Extensions.Sqlite;
using Server.Data;
using Server.Services;

namespace ServerTest;

[TestClass]
public class TransactionTest {
	const long EXODIA_THE_FORBIDDEN_ONE = 120;
	const long OTHER_YUGIOH_REFERENCE = 150;

	[ClassInitialize]
	public static void ClassInit(TestContext context) {
		MigrationService ms = new MigrationService();
		ms.RefreshTables<SqliteDatabase>();
	}

	[TestMethod]
	public void TestCreateTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction test = new(db, EXODIA_THE_FORBIDDEN_ONE);

			Assert.AreEqual(0, test.ID);

			test.Save();

			Assert.AreEqual(1, test.ID);
		}
	}

	[TestMethod]
	public void TestSelectTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction expected = new(db, EXODIA_THE_FORBIDDEN_ONE);
			expected.ID = 1;

			Transaction? result = Transaction.Find(1);

			Assert.AreEqual(expected, result);
		}
	}

	[TestMethod]
	public void TestSaveTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction transaction = Transaction.Find(1)!;
			transaction.Value = OTHER_YUGIOH_REFERENCE;

			transaction.Save(); // transaction.Update(); ?

			// How to assert this worked???
			Assert.AreEqual(1, transaction.ID);
		}
	}

	[Ignore]
	[TestMethod]
	public void TestUpdateTransaction() {
		using (SqliteDatabase db = new()) {
			//Transaction.Where('value', OTHER_YUGIOH_REFERENCE).Update('value', EXODIA_THE_FORBIDDEN_ONE);

			Transaction transaction = Transaction.Find(1)!;

			Assert.AreEqual(EXODIA_THE_FORBIDDEN_ONE, transaction.Value);
		}
	}
}