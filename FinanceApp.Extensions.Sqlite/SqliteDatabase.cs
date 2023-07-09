using FinanceApp.Abstractions;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;
using MSSqliteParameter = Microsoft.Data.Sqlite.SqliteParameter;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Extensions.Sqlite;

public class SqliteDatabase : IDatabase {
	private SqliteConnection DB = new("Data Source=test.db");
	public ConnectionState State => DB.State;

	public long? LastInsertId { get {
			string sql = @"SELECT LAST_INSERT_ROWID() LIMIT 1";
			SqliteCommand command = DB.CreateCommand();
			command.CommandText = sql;
			ParameterCollection parameters = ParameterCollection.Empty;
			command.Parameters.AddRange(parameters.ConvertParameters(Convert));

			long? count = (long?) command.ExecuteScalar();
			return count;
		} }

	public SqliteDatabase() {
		OpenConnection();
	}

	public IDatabase OpenConnection() {
		DB.Open();
		return this;
	}

	public List<T> ExecuteReader<T>(string sql, ParameterCollection vars) where T : new() {
		SqliteCommand command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ConvertParameters(Convert));

		List<T> result = new();

		using (Abstractions.IDataReader reader = new SqliteDataReader(command.ExecuteReader())) {
			Type type = typeof(T);
			PropertyInfo[] properties = type.GetProperties();
			Parser parser = new(reader, properties);
			result = parser.PerformParse<T>();
		}

		return result;
	}

	private MSSqliteParameter Convert(Parameter parameter) {
		MSSqliteParameter result = new(parameter.Name, (SqliteType) parameter.Type)
        {
            Value = parameter.Value
        };
        return result;
	}

	public int ExecuteNonQuery(string sql, ParameterCollection vars) {
		SqliteCommand command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ConvertParameters(Convert));
		return command.ExecuteNonQuery();
	}

	public void Dispose() {
		DB.Close();
	}
}

public static class SqliteDatabaseExtensions
{
    public static IServiceCollection AddSqliteDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDatabase, SqliteDatabase>();

        return services;
    }
}