using FinanceApp.DatabaseInterfaces;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;
using MSSqliteParameter = Microsoft.Data.Sqlite.SqliteParameter;

namespace FinanceApp.Extensions.Sqlite;

public class SqliteDatabase : IDatabase {
    private SqliteConnection DB = new SqliteConnection("Data Source=test.db");
    public ConnectionState State => DB.State;

    public SqliteDatabase()
    {
        OpenConnection();
    }

    public IDatabase OpenConnection()
    {
        DB.Open();
        return this;
    }

    public List<T> ExecuteReader<T>(string sql, ParameterCollection vars) where T : new()
    {
        SqliteCommand command = DB.CreateCommand();
        command.CommandText = sql;
		command.Parameters.AddRange(vars.ConvertParameters(p => Convert(p)));

        List<T> result = new();

        using (DatabaseInterfaces.IDataReader reader = new SqliteDataReader(command.ExecuteReader()))
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            /*Parser parser = new Parser(reader, type, properties);
            result = parser.PerformParse<T>();*/
        }

        return result;
    }

    private MSSqliteParameter Convert(Parameter parameter) {
        MSSqliteParameter result = new MSSqliteParameter(parameter.name, (SqliteType)parameter.type);
        result.Value = parameter.value;
        return result;
    }

    public int ExecuteNonQuery(string sql, ParameterCollection vars)
    {
        SqliteCommand command = DB.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(vars.ConvertParameters(p => Convert(p)));
        return command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        DB.Close();
    }
}
