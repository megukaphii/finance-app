using FinanceApp.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Server.Services;

public interface IMigrationService {
    void RefreshTables<TDatabase>() where TDatabase : IDatabase, new();
}

public class MigrationService : IMigrationService
{
    public void RefreshTables<TDatabase>() where TDatabase : IDatabase, new()
    {
        DropTables<TDatabase>();
        CreateTablesIfNotExists<TDatabase>();
	}

	private void DropTables<TDatabase>() where TDatabase : IDatabase, new() {
		Console.WriteLine("Dropping tables...");
		using (IDatabase db = new TDatabase()) {
			string sql =
			@"
				DROP TABLE IF EXISTS Transactions
			";
			db.ExecuteNonQuery(sql, ParameterCollection.Empty);
		}
        Console.WriteLine("Tables dropped.");
	}

	private void CreateTablesIfNotExists<TDatabase>() where TDatabase : IDatabase, new() {
		Console.WriteLine("Creating tables...");
		using (IDatabase db = new TDatabase())
        {
			// [TODO] Generate SQL with reflection on model classes? Will fail DatabaseTest as NOT NULL values will be set to NULL, maybe have simpler DB structure just for those tests?
            string sql =
            @"
				CREATE TABLE IF NOT EXISTS Transactions (
					ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
					Value INTEGER,
					Transactee TEXT
				);
			";
            db.ExecuteNonQuery(sql, ParameterCollection.Empty);
		}
		Console.WriteLine("Tables created.");
	}
}

public static class MigrationServiceExtensions
{
    public static IServiceCollection AddMigrationService(this IServiceCollection services)
    {
        services.AddSingleton<IMigrationService, MigrationService>();

        return services;
    }
}
