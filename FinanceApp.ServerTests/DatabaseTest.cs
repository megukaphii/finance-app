using FinanceApp.Extensions.Sqlite;
using Server.Data;
using Server.Services;

namespace ServerTest;

[TestClass]
public class TransactionTest {
	const long EXODIA_THE_FORBIDDEN_ONE = 120;

	[ClassInitialize]
	public static void ClassInit(TestContext context) {
		MigrationService ms = new MigrationService();
		ms.RefreshTables<SqliteDatabase>();
	}

	[TestMethod]
	public void TestCreateTransaction() {
		using (SqliteDatabase db = new()) {
			Transaction test = new Transaction(db, EXODIA_THE_FORBIDDEN_ONE).Save();

			Transaction saved = Transaction.Find(1);

			Assert.AreEqual(test, saved);
		}
	}
}