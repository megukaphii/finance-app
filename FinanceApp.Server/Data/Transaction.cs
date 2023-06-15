﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Classes;

namespace FinanceApp.Server.Data;

[Table("Transactions")]
public class Transaction : IEloquent<Transaction> {
	private readonly IDatabase Database;

	// Prevent ID being modified if exists on DB = true?
	[Column("ID")]
	public long ID { get; set; }
	[Column("Value")]
    public long Value { get; set; }
	[Column("Transactee")]
	public string Transactee { get; set; }

	public Transaction() {
		// [TODO] necessary for db.ExecuteReader, can we remove or at least get rid of the underline on it?
	}

	public Transaction(IDatabase database, long value, string transactee)
	{
		Database = database;
		Value = value;
		Transactee = transactee;
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}, Transactee: {Transactee}";
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
		using (SqliteDatabase db = new()) {
			// [TODO] This whole deal of converting columns into their DB equivalent and using that to fill out Parameters, should be refactored, maybe moved into another class even?
			// Something that can save a map of the properties to their DB types, and be thrown around - or do we want an Attribute to store DB type? Seems too manual, but would perform better?
			List<PropertyInfo> cols = new();

			Type type = typeof(Transaction);
			foreach (PropertyInfo prop in type.GetProperties()) {
				ColumnAttribute? nameAttr = (ColumnAttribute?) prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

				if (nameAttr?.Name != null) {
					cols.Add(prop);
				}
			}

			ParameterCollection parameters = new();

			foreach (PropertyInfo col in cols.Where(c => c.Name != "ID")) {
				SqlDbType sqlDbType;

				TypeCode typeCode = Type.GetTypeCode(col.PropertyType);
				switch (typeCode) {
					case TypeCode.Int64:
						sqlDbType = SqlDbType.Int;
						break;
					case TypeCode.Boolean:
						sqlDbType = SqlDbType.Bit;
						break;
					default:
						sqlDbType = SqlDbType.Text;
						break;
				}

				parameters.Add(new Parameter(sqlDbType, $"${col.Name}", col.GetValue(this)));
			}

			// [TODO] Both Update here, and the QueryBuilder below need a list of columns. Can we safely pass through this list here while being safe from SQL injection? Probably.
			// Also, is there a way/would it be useful to cache this data so we're not performing all these reflection operations on every query?
			string sql = QueryBuilder.Build<Transaction>().AsUpdate().ToString();

			try {
				// Using Database property fails test, because after Find() gets the model from DB, the Database property is never filled.
				db.ExecuteNonQuery(sql, parameters);
			} catch (Exception ex) {
				throw new Exception($"Query Failed! SQL: {sql}", ex);
			}

			return this;
		}
	}

	protected override Transaction Insert() {
		List<PropertyInfo> cols = new();

		Type type = typeof(Transaction);
		foreach (PropertyInfo prop in type.GetProperties()) {
			ColumnAttribute? nameAttr = (ColumnAttribute?) prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

			if (nameAttr?.Name != null) {
				cols.Add(prop);
			}
		}

		ParameterCollection parameters = new();

		foreach (PropertyInfo col in cols.Where(c => c.Name != "ID")) {
			SqlDbType sqlDbType;

			TypeCode typeCode = Type.GetTypeCode(col.PropertyType);
			switch (typeCode) {
				case TypeCode.Int64:
					sqlDbType = SqlDbType.Int;
					break;
				case TypeCode.Boolean:
					sqlDbType = SqlDbType.Bit;
					break;
				default:
					sqlDbType = SqlDbType.Text;
					break;
			}

			parameters.Add(new Parameter(sqlDbType, $"${col.Name}", col.GetValue(this)));
		}

		string sql = QueryBuilder.Build<Transaction>().AsInsert().ToString();

		try {
			Database.ExecuteNonQuery(sql, parameters);
		} catch (Exception ex) {
			throw new Exception($"Query Failed! SQL: {sql}", ex);
		}

		// This should be fine right?
		ID = (long) Database.LastInsertId!;

		existsOnDb = true;

		return this;
	}
}