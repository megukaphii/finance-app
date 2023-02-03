namespace FinanceApp.DatabaseInterfaces;

public interface IDataReader : IDisposable
{
    public int FieldCount { get; }

    public bool Read();

    public Type GetFieldType(int columnIndex);

    public string GetName(int columnIndex);

    public object GetValue(int columnIndex);
}
