using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace FinanceApp.Server.Classes;

public class QueryProperty {
	public PropertyInfo Property { get; set; }
	public string Name { get; set; }
}

public class QueryBuilder {
	public string TableName { get; set; } = string.Empty;
	public List<string> Columns { get; set; } = new List<string>();
	public string Query { get; set; } = string.Empty;

	public List<QueryProperty> KeyProperties { get; set; } = new List<QueryProperty>();
	public List<QueryProperty> NonKeyProperties { get; set; } = new List<QueryProperty>();
	public List<QueryProperty> AllProperties { get; set; } = new List<QueryProperty>();

	public override string ToString() {
		return Query;
	}

	public QueryBuilder AsSelect() {
		StringBuilder sb = new StringBuilder();

		sb.Append("SELECT ");

		sb.Append(string.Join(", ", Columns));
		
		sb.Append(" FROM ").Append(TableName);

		/*if (includeKey) {
			sb.Append(" WHERE ");
			sb.Append(string.Join(" AND ", KeyProperties.Select(x => $"{x.Name} = @{x.Name}")));
		}*/

		Query = sb.ToString();
		return this;
	}

	public QueryBuilder AsInsert() {
		StringBuilder sb = new StringBuilder();

		sb.Append($"INSERT INTO {TableName} (");
		// [TODO] Should we store the ID column separately? Probably?
		sb.Append(string.Join(", ", Columns.Where(x => x != "ID")));
		sb.Append(") VALUES (");
		// Is this safe, or should we grab the column name and stick a dollar sign in front of it?
		sb.Append(string.Join(", ", Columns.Where(x => x != "ID").Select(x => $"${x}")));
		sb.Append(")");

		Query = sb.ToString();

		return this;
	}

	public QueryBuilder AsUpdate() {
		StringBuilder sb = new StringBuilder();

		sb.Append($"UPDATE {TableName} SET ");

		foreach (string col in Columns.Where(x => x != "ID")) {
			sb.Append($"{col} = ${col}");
		}

		Query = sb.ToString();

		return this;
	}

	public QueryBuilder Where(string column, object value) {
		StringBuilder sb = new StringBuilder(Query);

		// Would break if ID was extracted to its' own property
		if (!Columns.Contains(column)) {
			throw new ArgumentException($"Column {column} does not exist or is not accessible on {TableName}!");
		}

		sb.Append($" WHERE {column} = $ID");

		Query = sb.ToString();

		return this;
	}

	/*public string AsInsert(bool explicitColumns = true) {
		var sb = new StringBuilder();
		sb.Append("INSERT INTO ").Append(TableName).Append(' ');

		if (explicitColumns) {
			sb.Append('(');
			sb.Append(string.Join(", ", AllProperties.Select(x => x.Name)));
			sb.Append(')');
		}

		sb.Append(" VALUES (");
		sb.Append(string.Join(", ", AllProperties.Select(x => $"@{x.Name}")));
		sb.Append(')');

		return sb.ToString();
	}

	public string AsUpdate(bool explicitColumns = true, bool includeKey = true) {
		var sb = new StringBuilder();
		sb.Append("UPDATE ").Append(TableName).Append(' ');

		sb.Append("SET ");
		sb.Append(string.Join(", ", NonKeyProperties.Select(x => $"{x.Name} = @{x.Name}")));

		if (includeKey) {
			sb.Append(" WHERE ");
			sb.Append(string.Join(" AND ", KeyProperties.Select(x => $"{x.Name} = @{x.Name}")));
		}

		return sb.ToString();
	}*/

	public static QueryBuilder Build<T>(IEnumerable<string> additionalConstraints = null) {
		var result = new QueryBuilder();

		var type = typeof(T);

		var attr = (TableAttribute?) type.GetCustomAttribute(typeof(TableAttribute), true);

		if (attr != null) {
			result.TableName = attr.Name;
		} else {
			throw new ArgumentException($"{type.Name} does not represent a DB table");
		}

		foreach (var prop in type.GetProperties()) {
			// [TODO] Can we just get the list of properties with the column attribute, then use the property name, rather than the name provided to the attribute?
			var nameAttr = (ColumnAttribute?) prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

			if (nameAttr?.Name != null) {
				result.Columns.Add(nameAttr.Name);
			}
		}

		return result;
	}
}