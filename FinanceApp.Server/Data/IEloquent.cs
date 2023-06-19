using System.ComponentModel.DataAnnotations.Schema;
using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Classes;

namespace FinanceApp.Server.Data;

public abstract class Eloquent<T> where T : Eloquent<T>, new() {
    protected bool ExistsOnDb;
    
    private long id;
    
    [Column("ID")]
    public long ID {
        get => id;
        set
        {
            if (!ExistsOnDb) {
                id = value;
            } else {
                throw new Exception("Cannot assign ID to existing DB entry");
            }
        }
    }

	public T Save()
    {
        return ExistsOnDb ? Update() : Insert();
    }

	protected abstract T Update();

	protected abstract T Insert();

	public static T? Find(int id)
    {
		// [TODO] Given that these are static methods, how do we inject the appropriate DB?
		using (SqliteDatabase db = new()) {
			ParameterCollection parameters = new() {
				new Parameter(System.Data.SqlDbType.Int, "$ID", id)
			};
			string sql = QueryBuilder.Build<T>().AsSelect().Where("ID", id).ToString();

			T? result = null;
			try {
				// Do what when element not found? Return null/default value, or throw exception? Null is bad, but an exception seems extreme.
				result = db.ExecuteReader<T>(sql, parameters).First();
			} catch (Exception e) {
				throw new Exception($"Query Failed! SQL: {sql}", e);
			}

			result.ExistsOnDb = true;

			return result;
		}
	}

	public static List<T> All()
    {
		using (SqliteDatabase db = new()) {
			string sql = QueryBuilder.Build<T>().AsSelect().ToString();
			return db.ExecuteReader<T>(sql, ParameterCollection.Empty);
		}
	}
}
