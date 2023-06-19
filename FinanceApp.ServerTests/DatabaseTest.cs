using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Data;
using FinanceApp.Server.Services;

namespace FinanceApp.ServerTests;

[TestClass]
public class DatabaseTest {
	[ClassInitialize]
	public static void ClassInit(TestContext context) {
		MigrationService ms = new();
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
    public void TestExecuteNonQueryWithNullParam()
    {
        int rowsUpdated;
        using (SqliteDatabase db = new()) {
            string sql =
            @"
			    INSERT INTO Transactions (
				    Transactee
			    )
			    VALUES (
				    $transactee
			    );
		    ";
            
            ParameterCollection parameters = new() {
                new Parameter(System.Data.SqlDbType.Text, "$transactee", DBNull.Value)
            };
            rowsUpdated = db.ExecuteNonQuery(sql, parameters);
        }
        
        Assert.AreEqual(1, rowsUpdated);
    }

	[TestMethod]
	public void TestExecuteReader() {
		MigrationService ms = new();
		ms.RefreshTables<SqliteDatabase>();
		InsertIntoTransactionsWithParams(250);
		InsertIntoTransactionsWithParams(125);

		using (SqliteDatabase db = new()) {
			string sql =
			@"
				SELECT ID, Value
				FROM Transactions
			";
			List<Transaction> transactions = db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
			Assert.AreEqual(2, transactions.Count);
		}
	}

    [TestMethod]
    public void TestExecuteReaderWithNullResult() {
        MigrationService ms = new();
        ms.RefreshTables<SqliteDatabase>();
        InsertIntoTransactionsWithParams(250);

        using (SqliteDatabase db = new()) {
            string sql =
                @"
				SELECT Transactee
				FROM Transactions
				WHERE ID = 1
			";
            List<Transaction> transactions = db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
            Assert.IsNull(transactions[0].Transactee);
        }
    }

    [TestMethod]
	public void TestExecuteReaderWithParams() {
		MigrationService ms = new();
		ms.RefreshTables<SqliteDatabase>();
		InsertIntoTransactionsWithParams(250);
		InsertIntoTransactionsWithParams(125);

		using (SqliteDatabase db = new()) {
			string sql =
			@"
				SELECT ID, Value
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