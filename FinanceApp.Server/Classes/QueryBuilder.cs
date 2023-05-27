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

	public List<QueryProperty> KeyProperties { get; set; } = new List<QueryProperty>();
	public List<QueryProperty> NonKeyProperties { get; set; } = new List<QueryProperty>();
	public List<QueryProperty> AllProperties { get; set; } = new List<QueryProperty>();

	public string AsSelect() {
		var sb = new StringBuilder();
		sb.Append("SELECT ");

		sb.Append(string.Join(", ", Columns));
		
		sb.Append(" FROM ").Append(TableName);

		/*if (includeKey) {
			sb.Append(" WHERE ");
			sb.Append(string.Join(" AND ", KeyProperties.Select(x => $"{x.Name} = @{x.Name}")));
		}*/

		return sb.ToString();
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
			var nameAttr = (ColumnAttribute?) prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();

			// Will this work?
			if (nameAttr?.Name != null) {
				result.Columns.Add(nameAttr.Name);
			}
		}

		return result;

		/*var keys = result.AllProperties.Where(x => x.Property.IsDefined(typeof(KeyAttribute), true));
		if (keys.Any()) {
			foreach (var key in keys) {
				result.KeyProperties.Add(key);
			}
		}

		var nonKeys = result.AllProperties.Where(x => !x.Property.IsDefined(typeof(KeyAttribute), true));
		if (nonKeys.Any()) {
			foreach (var nonKey in nonKeys) {
				result.NonKeyProperties.Add(nonKey);
			}
		}

		if (additionalConstraints?.Any() == true) {
			foreach (var keyName in additionalConstraints) {
				var key = result.AllProperties.Where(x => x.Property.Name == keyName).FirstOrDefault();
				if (key == null) {
					throw new ArgumentException($"The value '{key}' provided for '{nameof(key)}' was not a property of the type '{type.FullName}'.");
				}

				result.KeyProperties.Add(key);
			}
		}

		return result;*/
	}
}