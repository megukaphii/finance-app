using Server.Classes;
using Server.Data;
using Server.Database;

Migration.RefreshTables();
Seeder.SeedDB();
List<Transaction> transactions = Transaction.All();
foreach (Transaction t in transactions) {
	Console.WriteLine(t);
}

FinanceServer server = new();
await server.Start();
Console.ReadKey();