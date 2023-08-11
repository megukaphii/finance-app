using System.Reflection;

namespace FinanceApp.Abstractions;

public class Parser
{
    private readonly IDatabase _database;
    private readonly IDataReader _reader;
    private readonly PropertyInfo[] _properties;

    public Parser(IDatabase database, IDataReader reader, PropertyInfo[] properties)
    {
        _database = database;
        _reader = reader;
        _properties = properties;
    }

    public List<T> PerformParse<T>() where T : Eloquent, new()
    {
        List<T> result = new();
        while (_reader.Read()) {
            result.Add(ParseRow<T>());
        }
        return result;
    }

    private T ParseRow<T>() where T : Eloquent, new()
    {
        T instance = new();
        for (int i = 0; i < _reader.FieldCount; i++) {
            ParseField(instance, i);
		}
		instance.Database = _database;
		instance.ExistsOnDb = true;
        return instance;
    }

    private void ParseField<T>(T instance, int fieldIdx)
    {
        string fieldName = _reader.GetName(fieldIdx);
        PropertyInfo property = GetProperty<T>(fieldName);
        SetValue(property, instance, _reader.GetValue(fieldIdx));
    }

    private PropertyInfo GetProperty<T>(string fieldName)
    {
        try {
            return _properties.Single(p => p.Name == fieldName);
        } catch (Exception e) {
            throw new Exception($"{typeof(T)} doesn't contain field {fieldName}", e);
        }
    }

    private static void SetValue<T>(PropertyInfo property, T instance, object value)
    {
        property.SetValue(instance, value is DBNull ? null : value);
    }
}