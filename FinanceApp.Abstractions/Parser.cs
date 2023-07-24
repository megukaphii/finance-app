using System.Reflection;

namespace FinanceApp.Abstractions;

public class Parser
{
    private readonly IDatabase database;
    private readonly IDataReader reader;
    private readonly PropertyInfo[] properties;

    public Parser(IDatabase database, IDataReader reader, PropertyInfo[] properties)
    {
        this.database = database;
        this.reader = reader;
        this.properties = properties;
    }

    public List<T> PerformParse<T>() where T : Eloquent, new()
    {
        List<T> result = new();
        while (reader.Read()) {
            result.Add(ParseRow<T>());
        }
        return result;
    }

    private T ParseRow<T>() where T : Eloquent, new()
    {
        T instance = new();
        for (int i = 0; i < reader.FieldCount; i++) {
            ParseField(instance, i);
		}
		instance.Database = database;
		instance.ExistsOnDb = true;
        return instance;
    }

    private void ParseField<T>(T instance, int fieldIdx)
    {
        string fieldName = reader.GetName(fieldIdx);
        PropertyInfo property = GetProperty<T>(fieldName);
        SetValue(property, instance, reader.GetValue(fieldIdx));
    }

    private PropertyInfo GetProperty<T>(string fieldName)
    {
        try {
            return properties.Single(p => p.Name == fieldName);
        } catch (Exception e) {
            throw new Exception($"{typeof(T)} doesn't contain field {fieldName}", e);
        }
    }

    private static void SetValue<T>(PropertyInfo property, T instance, object value)
    {
        property.SetValue(instance, value is DBNull ? null : value);
    }
}