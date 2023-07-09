using System.Data;

namespace FinanceApp.Abstractions;

public interface IDatabase : IDisposable {
	ConnectionState State { get; }
	long? LastInsertId { get; }

	IDatabase OpenConnection();

	int ExecuteNonQuery(string sql, ParameterCollection vars);

	List<T> ExecuteReader<T>(string sql, ParameterCollection vars) where T : Eloquent<T>, new();
}