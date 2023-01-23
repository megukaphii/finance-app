using Server.Classes;

FinanceServer server = new();
await server.Start();
Console.ReadKey();