using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using FinanceApp.Abstractions;

namespace FinanceApp.Data.Models;

[Table("Transactions")]
public class Transaction : Eloquent {
	[Column("Value")]
    public long Value { get; set; }
	[Column("Transactee")]
	public string Transactee { get; set; } = string.Empty;

    public Transaction() {}

    public Transaction(IDatabase database, long value, string transactee)
	{
		Database = database;
		Value = value;
		Transactee = transactee;
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}, Transactee: {Transactee}";
	}

	public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

        Transaction other = (Transaction) obj;
        return ID == other.ID && Value == other.Value;
    }

	// It complains if we don't override this too
	public override int GetHashCode() {
		throw new NotImplementedException();
	}

	protected override Transaction Update() {
        ParameterCollection parameters = GetDBParams<Transaction>(this);

		// [TODO] Both Update here, and the QueryBuilder below need a list of columns. Can we safely pass through this list here while being safe from SQL injection? Probably.
		// Also, is there a way/would it be useful to cache this data so we're not performing all these reflection operations on every query?
		string sql = QueryBuilder.Build<Transaction>().AsUpdate().ToString();

		try {
			Database?.ExecuteNonQuery(sql, parameters);
		} catch (Exception ex) {
			throw new Exception($"Query Failed! SQL: {sql}", ex);
		}

		return this;
	}

	protected override Transaction Insert() {
        ParameterCollection parameters = GetDBParams<Transaction>(this);

		string sql = QueryBuilder.Build<Transaction>().AsInsert().ToString();

		try {
			Database?.ExecuteNonQuery(sql, parameters);
		} catch (Exception ex) {
			throw new Exception($"Query Failed! SQL: {sql}", ex);
		}

		// This should be fine right?
		ID = Database?.LastInsertId ?? -1;

		ExistsOnDb = true;

		return this;
	}
}