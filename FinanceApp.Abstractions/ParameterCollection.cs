using System.Collections;
using System.Data;

namespace FinanceApp.Abstractions;

public class ParameterCollection : IEnumerable<Parameter>
{
    public static readonly ParameterCollection Empty = new();

    private readonly List<Parameter> parameters = new();

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public IEnumerator<Parameter> GetEnumerator() { return parameters.GetEnumerator(); }

    public void Add(Parameter p) => parameters.Add(p);

    public List<T> ConvertParameters<T>(Func<Parameter, T> sqlParameterAdapterFactory)
    {
        return parameters.Select(sqlParameterAdapterFactory).ToList();
    }
}

public class Parameter
{
    public readonly SqlDbType Type;
    public readonly string Name;
    public readonly object? Value;

    public Parameter(SqlDbType type, string name, object? value)
    {
        Type = type;
        Name = name;
        Value = value ?? DBNull.Value;
    }
}