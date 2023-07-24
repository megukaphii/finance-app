using FinanceApp.Extensions.Sqlite;
using FinanceApp.Server.Classes;
using FinanceApp.Server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((_, configBuilder) =>
    {

    })
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<IServer, FinanceServer>();
        services.AddMigrationService();
        services.AddSqliteDatabase();
        // Add IDatabase Dependencies here
        // TODO: IDatabase is IDisposable - figure out object lifetimes
        // The IDatabase should only really be closed when the application is closed, I think?
        // True, but finding a "OnClosing" Event for DI containers is a funny story. I'd like it to
        // look elegant enough at least without us piecing together funny code lol
        // Oh dear, right.
        // TODO: Add extension methods for adding each implementation of IDatabase for readability

        //services.AddSingleton<
    })
.Build();

IServer server = host.Services.GetRequiredService<IServer>();

IMigrationService ms = host.Services.GetRequiredService<IMigrationService>();
ms.RefreshTables<SqliteDatabase>();

await server.Start();