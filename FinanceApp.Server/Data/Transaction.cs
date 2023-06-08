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

	// Prevent ID being modified if exists on DB = true?
	[Column("ID")]
	public long ID { get; set; }
	[Column("Value")]
    public long Value { get; set; }

	public Transaction() {
		// [TODO] necessary for db.ExecuteReader
	}

	public Transaction(IDatabase database, long value)
	{
		Database = database;
		Value = value;
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}";
	}

	// [TODO] Can we pull this up to IEloquent? Value == other.Value and similar model-specific comparisons like that would be the only problem, but reflection could fix that? I think?
	public override bool Equals(object? obj) {
		if ((obj == null) || !GetType().Equals(obj.GetType())) {
			return false;
		} else {
			Transaction other = (Transaction) obj;
			return (ID == other.ID) && (Value == other.Value);
		}
	}

	// It complains if we don't override this too
	public override int GetHashCode() {
		throw new NotImplementedException();
	}

	protected override Transaction Update() {
		throw new NotImplementedException();
	}

	protected override Transaction Insert() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$Value", Value)
			};
			string sql = QueryBuilder.Build<Transaction>().AsInsert().ToString();

			try {
				db.ExecuteNonQuery(sql, parameters);
			} catch (Exception ex) {
				throw new Exception($"Query Failed! SQL: {sql}", ex);
			}

			// This should be fine right?
			ID = (long) db.LastInsertId!;

			existsOnDb = true;

			return this;
		}
	}
}