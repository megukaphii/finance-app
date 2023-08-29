using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Data.OldModels;
using FinanceApp.Server.Services;
using NUnit.Framework;

namespace FinanceApp.ServerTests;

[TestFixture]
public class DatabaseTest {
	[OneTimeSetUp]
	public static void OneTimeSetUp() {
		MigrationService ms = new();
		ms.RefreshTables<SqliteDatabase>();
	}

	[Test]
	public void TestOpenClose() {
		SqliteDatabase db = new();
		db.OpenConnection();
		Assert.IsTrue(db.State == System.Data.ConnectionState.Open);
		db.Dispose();
		Assert.IsTrue(db.State == System.Data.ConnectionState.Closed);
	}

	[Test]
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

	[Test]
	public void TestExecuteNonQueryWithParams() {
		int rowsUpdated = InsertIntoTransactionsWithParams(125);
		Assert.AreEqual(1, rowsUpdated);
	}

    [Test]
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
                new Parameter(System.Data.SqlDbType.Text, "$transactee", null)
            };
            rowsUpdated = db.ExecuteNonQuery(sql, parameters);
        }
        
        Assert.AreEqual(1, rowsUpdated);
    }

	[Test]
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

    [Test]
    public void TestExecuteReaderWithNullField() {
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

    [Test]
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