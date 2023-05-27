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

	public static List<Transaction> All() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			// [TODO] Better way to do this?
			string sql = QueryBuilder.Build<Transaction>().AsSelect().ToString();
			return db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
		}
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}";
	}

	public override bool Equals(object? obj) {
		if ((obj == null) || !GetType().Equals(obj.GetType())) {
			return false;
		} else {
			Transaction other = (Transaction) obj;
			return (ID == other.ID) && (Value == other.Value);
		}
	}

	public Transaction Save() {
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

			return this;
		}
	}

	public static Transaction? Find(int id) {
		using (SqliteDatabase db = new SqliteDatabase()) {
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$ID", id)
			};
			string sql = QueryBuilder.Build<Transaction>().AsSelect().Where("ID", id).ToString();

			Transaction? result = null;
			try {
				result = db.ExecuteReader<Transaction>(sql, parameters).FirstOrDefault();
			} catch (Exception ex) {
				throw new Exception($"Query Failed! SQL: {sql}", ex);
			}

			return result;
		}
	}

	// It complains if we don't override this too
	public override int GetHashCode() {
		throw new NotImplementedException();
	}
}