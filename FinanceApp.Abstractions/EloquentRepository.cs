namespace FinanceApp.Abstractions;

public class EloquentRepository<T> : IEloquentRepository<T> where T : Eloquent<T>, new()
{
    IDatabase db;

    public EloquentRepository(IDatabase db)
    {
        this.db = db;
    }

    public T Find(int id)
    {
        ParameterCollection parameters = new() {
            new Parameter(System.Data.SqlDbType.Int, "$ID", id)
        };
        string sql = QueryBuilder.Build<T>().AsSelect().Where("ID", id).ToString();

        T? result;
        try
        {
            // Do what when element not found? Return null/default value, or throw exception? Null is bad, but an exception seems extreme.
            result = db.ExecuteReader<T>(sql, parameters).First();
        }
        catch (Exception e)
        {
            throw new Exception($"Query Failed! SQL: {sql}", e);
        }

        return result;
    }

    public List<T> All()
    {
        string sql = QueryBuilder.Build<T>().AsSelect().ToString();
        return db.ExecuteReader<T>(sql, ParameterCollection.Empty);
    }
}