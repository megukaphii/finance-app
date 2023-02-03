﻿using Microsoft.Extensions.DependencyInjection;
using FinanceApp.DatabaseInterfaces;

namespace Server.Services;

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

    private void CreateTablesIfNotExists<TDatabase>() where TDatabase : IDatabase, new()
    {
        using (IDatabase db = new TDatabase())
        {
            string sql =
            @"
				CREATE TABLE IF NOT EXISTS Transactions (
					ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
					Value INTEGER
				);
			";
            db.ExecuteNonQuery(sql, ParameterCollection.Empty);
        }
    }

    private void DropTables<TDatabase>() where TDatabase : IDatabase, new()
    {
        using (IDatabase db = new TDatabase())
        {
            string sql =
            @"
				DROP TABLE IF EXISTS Transactions
			";
            db.ExecuteNonQuery(sql, ParameterCollection.Empty);
        }
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
