using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace FinanceApp.Server.Classes;

public class QueryBuilder
{
    private string TableName { get; set; } = string.Empty;
    private List<string> AllColumns => Columns.Concat(KeyColumns).ToList();
    private List<string> KeyColumns { get; } = new();
    private List<string> Columns { get; } = new();

    private string Query { get; set; } = string.Empty;

    public override string ToString() { return Query; }

    public QueryBuilder AsSelect()
    {
        StringBuilder sb = new();

        sb.Append("SELECT ");

        sb.Append(string.Join(", ", AllColumns));

        sb.Append(" FROM ").Append(TableName);

        Query = sb.ToString();
        return this;
    }

    public QueryBuilder AsInsert()
    {
        StringBuilder sb = new();

        sb.Append($"INSERT INTO {TableName} (");
        sb.Append(string.Join(", ", Columns));
        sb.Append(") VALUES (");
        sb.Append(string.Join(", ", Columns.Select(x => $"${x}")));
        sb.Append(')');

        Query = sb.ToString();

        return this;
    }

    public QueryBuilder AsUpdate()
    {
        StringBuilder sb = new();

        sb.Append($"UPDATE {TableName} SET ");

        List<string> setCols = Columns
            .Select(x => $"{x} = ${x}")
            .ToList();
        sb.Append(string.Join(", ", setCols));

        Query = sb.ToString();

        return this;
    }

    public QueryBuilder Where(string column, object value)
    {
        StringBuilder sb = new(Query);

        if (!AllColumns.Contains(column)) {
            throw new ArgumentException($"Column {column} does not exist or is not accessible on {TableName}!");
        }

        sb.Append($" WHERE {column} = ${column}");

        Query = sb.ToString();

        return this;
    }

    public static QueryBuilder Build<T>()
    {
        QueryBuilder result = new();
        Type type = typeof(T);
        TableAttribute? attr = (TableAttribute?)type.GetCustomAttribute(typeof(TableAttribute), true);

        if (attr != null) {
            result.TableName = attr.Name;
        }
        else {
            throw new ArgumentException($"{type.Name} does not represent a DB table");
        }

        foreach (PropertyInfo prop in type.GetProperties()) {
            bool hasColumnAttribute = prop.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0;

            if (!hasColumnAttribute) continue;
            
            if (prop.Name.Contains("ID")) {
                result.KeyColumns.Add(prop.Name);
            } else {
                result.Columns.Add(prop.Name);
            }
        }

        return result;
    }
}