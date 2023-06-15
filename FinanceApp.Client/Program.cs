using FinanceApp.Client.Classes;

FinanceClient client = new();
await client.Start();
Console.ReadKey();