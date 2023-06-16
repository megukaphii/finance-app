using FinanceApp.Abstractions;
using MSSqliteDataReader = Microsoft.Data.Sqlite.SqliteDataReader;

namespace FinanceApp.Extensions.Sqlite;

public class SqliteDataReader : IDataReader {
    private readonly MSSqliteDataReader dataReader;

	public int FieldCount => dataReader.FieldCount;

	public SqliteDataReader(MSSqliteDataReader dataReader) {
		this.dataReader = dataReader;
	}

	public bool Read() {
		return dataReader.Read();
	}

	public Type GetFieldType(int columnIdx) {
		return dataReader.GetFieldType(columnIdx);
	}

	public string GetName(int columnIdx) {
		return dataReader.GetName(columnIdx);
	}

	public object GetValue(int columnIndex) {
		return dataReader.GetValue(columnIndex);
	}

	public void Dispose() {
        GC.SuppressFinalize(this);
		dataReader.Dispose();
	}
}
