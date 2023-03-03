using FinanceApp.Extensions.Sqlite;
using Server.Classes;
using Server.Data;
using Server.Database;
using Server.Services;

//MigrationService.RefreshTables();
/*Seeder.SeedDB();
List<Transaction> transactions = Transaction.All();
foreach (Transaction t in transactions) {
	Console.WriteLine(t);
}*/
MigrationService ms = new MigrationService();
ms.RefreshTables<SqliteDatabase>();

FinanceServer server = new();
await server.Start();
Console.ReadKey();