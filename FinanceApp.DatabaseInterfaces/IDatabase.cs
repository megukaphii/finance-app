using System.Data;

namespace FinanceApp.DatabaseInterfaces;

public interface IDatabase : IDisposable {
	ConnectionState State { get; }

	IDatabase OpenConnection();

	int ExecuteNonQuery(string sql, ParameterCollection vars);

	List<T> ExecuteReader<T>(string sql, ParameterCollection vars) where T : new();
}