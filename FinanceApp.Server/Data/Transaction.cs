using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Classes;
using FinanceApp.Server.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data;

[Table("Transactions")]
public class Transaction : IEloquent<Transaction> {
	// MONEY MONEY MONEY MONEY MONEY TRANSACTION MONEY MONEY I believe that's how banks work

	private readonly IDatabase Database;

	[Column]
	public long ID { get; set; }
	[Column]
    public long Value { get; set; }

	public Transaction() {
		// necessary for db.ExecuteReader
	}

	public Transaction(IDatabase database, long value)
	{
		Database = database;
		Value = value;
	}

	public static List<Transaction> All() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			// Better way to do this?
			string sql = QueryBuilder.Build<Transaction>().AsSelect().ToString();
			return db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
		}
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}";
	}

	public Transaction Save() {
		return this;
		throw new NotImplementedException();
	}

	public static Transaction Find(int id) {
		throw new NotImplementedException();
	}
}