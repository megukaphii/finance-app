using Microsoft.Data.Sqlite;
using System.Reflection;

namespace Server.Database;

public class Database : IDisposable {
	private SqliteConnection DB = new SqliteConnection("Data Source=test.db");

	public Database() {
		OpenConnection();
	}

	public Database OpenConnection() {
		DB.Open();
		return this;
	}

	public List<T> ExecuteReader<T>(string sql, Parameters vars) where T : new() {
		var command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ToSqliteParameterEnumerable());

		List<T> result = new();

		using (SqliteDataReader reader = command.ExecuteReader()) {
			Type type = typeof(T);
			PropertyInfo[] properties = type.GetProperties();
			Parser parser = new Parser(reader, type, properties);
			result = parser.PerformParse<T>();
		}

		return result;
	}

	public Database ExecuteNonQuery(string sql, Parameters vars) {
		var command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ToSqliteParameterEnumerable());
		command.ExecuteNonQuery();
		return this;
	}

	public void Dispose() {
		DB.Close();
	}
}
