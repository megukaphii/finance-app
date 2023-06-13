using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using Server.Data;
using Server.Services;

namespace ServerTest;

[TestClass]
public class DatabaseTest {
	[ClassInitialize]
	public static void ClassInit(TestContext context) {
		MigrationService ms = new MigrationService();
		ms.RefreshTables<SqliteDatabase>();
	}

	[TestMethod]
	public void TestOpenClose() {
		SqliteDatabase db = new();
		db.OpenConnection();
		Assert.IsTrue(db.State == System.Data.ConnectionState.Open);
		db.Dispose();
		Assert.IsTrue(db.State == System.Data.ConnectionState.Closed);
	}

	[TestMethod]
	public void TestExecuteNonQuery() {
		using (SqliteDatabase db = new()) {
			string sql =
			@"
				INSERT INTO Transactions (
					Value
				)
				VALUES (
					250
				);
			";

			int rowsUpdated = db.ExecuteNonQuery(sql, ParameterCollection.Empty);
			Assert.AreEqual(1, rowsUpdated);
		}
	}

	[TestMethod]
	public void TestExecuteNonQueryWithParams() {
		int rowsUpdated = InsertIntoTransactionsWithParams(125);
		Assert.AreEqual(1, rowsUpdated);
	}

	[TestMethod]
	public void TestExecuteReader() {
		MigrationService ms = new MigrationService();
		ms.RefreshTables<SqliteDatabase>();
		InsertIntoTransactionsWithParams(250);
		InsertIntoTransactionsWithParams(125);

		using (SqliteDatabase db = new()) {
			string sql =
			@"
				SELECT *
				FROM Transactions
			";
			List<Transaction> transactions = db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
			Assert.AreEqual(2, transactions.Count);
		}
	}

	// [TODO] Test reader with NULL result

	[TestMethod]
	public void TestExecuteReaderWithParams() {
		MigrationService ms = new MigrationService();
		ms.RefreshTables<SqliteDatabase>();
		InsertIntoTransactionsWithParams(250);
		InsertIntoTransactionsWithParams(125);

		using (SqliteDatabase db = new()) {
			string sql =
			@"
				SELECT *
				FROM Transactions
				WHERE Value = $value
			";
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$value", 125)
			};
			List<Transaction> transactions = db.ExecuteReader<Transaction>(sql, parameters);
			Assert.AreEqual(1, transactions.Count);
		}
	}

	private int InsertIntoTransactionsWithParams(int value) {
		using (SqliteDatabase db = new()) {
			string sql =
			@"
				INSERT INTO Transactions (
					Value
				)
				VALUES (
					$value
				);
			";
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$value", value)
			};
			return db.ExecuteNonQuery(sql, parameters);
		}
	}
}