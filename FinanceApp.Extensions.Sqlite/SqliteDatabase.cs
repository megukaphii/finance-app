using FinanceApp.Abstractions;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;
using MSSqliteParameter = Microsoft.Data.Sqlite.SqliteParameter;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Extensions.Sqlite;

public class SqliteDatabase : IDatabase
{
    private readonly SqliteConnection _db = new("Data Source=test.db");
    private readonly IParser _parser;
    public ConnectionState State => _db.State;

    public long LastInsertId
    {
        get
        {
            const string sql = @"SELECT LAST_INSERT_ROWID() LIMIT 1";
            SqliteCommand command = _db.CreateCommand();
            command.CommandText = sql;
            ParameterCollection parameters = ParameterCollection.Empty;
            command.Parameters.AddRange(parameters.ConvertParameters(Convert));

            long? result = (long?)command.ExecuteScalar();
            return result ?? -1;
        }
    }

    public SqliteDatabase() { }

    public SqliteDatabase(IParser parser)
    {
        _parser = parser;
        OpenConnection();
    }

    public IDatabase OpenConnection()
    {
        _db.Open();
        return this;
    }

    public List<T> ExecuteReader<T>(string sql, ParameterCollection vars) where T : Eloquent, new()
    {
        SqliteCommand command = _db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(vars.ConvertParameters(Convert));

        using Abstractions.IDataReader reader = new SqliteDataReader(command.ExecuteReader());
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        List<T> result = _parser.PerformParse<T>();

        return result;
    }

    private static MSSqliteParameter Convert(Parameter parameter)
    {
        MSSqliteParameter result = new(parameter.Name, (SqliteType)parameter.Type)
        {
            Value = parameter.Value
        };
        return result;
    }

    public int ExecuteNonQuery(string sql, ParameterCollection vars)
    {
        SqliteCommand command = _db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(vars.ConvertParameters(Convert));
        return command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _db.Close();
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