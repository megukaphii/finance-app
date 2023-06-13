using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Classes;

namespace FinanceApp.Server.Data;

public abstract class IEloquent<T> where T : IEloquent<T>, new() {
	public bool existsOnDb = false;

	public T Save() {
		if (existsOnDb) {
			return Update();
		} else {
			return Insert();
		}
	}

	protected abstract T Update();

	protected abstract T Insert();

	public static T? Find(int id){
		// [TODO] Given that these are static methods, how do we inject the appropriate DB?
		using (SqliteDatabase db = new SqliteDatabase()) {
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$ID", id)
			};
			string sql = QueryBuilder.Build<T>().AsSelect().Where("ID", id).ToString();

			T? result = null;
			try {
				// Do what when element not found? Return null/default value, or throw exception? Null is bad, but an exception seems extreme.
				result = db.ExecuteReader<T>(sql, parameters).First();
			} catch (Exception ex) {
				throw new Exception($"Query Failed! SQL: {sql}", ex);
			}

			result.existsOnDb = true;

			return result;
		}
	}

	public static List<T> All() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			// [TODO] Better way to do this? Also need to set exists on DB - is there a way to do that en-masse other than mapping over the array? Something relating to the actual construction of the object instead?
			string sql = QueryBuilder.Build<T>().AsSelect().ToString();
			return db.ExecuteReader<T>(sql, ParameterCollection.Empty);
		}
	}
}
