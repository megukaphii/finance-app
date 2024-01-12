using FinanceApp.Data;
using FinanceApp.Data.Models;
using FinanceApp.ServerTests.Helpers;

namespace FinanceApp.ServerTests.Extensions;

public static class FinanceAppContextExtensions
{
	public static Account[] LoadAccounts(this FinanceAppContext context)
	{
		if (context.Accounts.Any()) return context.Accounts.ToArray();

		Account[] accounts = DatabaseSeeder.Accounts;
		context.Accounts.AddRange(accounts);
		context.SaveChanges();
		return accounts;
	}

	public static Counterparty[] LoadCounterparties(this FinanceAppContext context)
	{
		if (context.Counterparties.Any()) return context.Counterparties.ToArray();

		Counterparty[] counterparties = DatabaseSeeder.Counterparties;
		context.Counterparties.AddRange(counterparties);
		context.SaveChanges();
		return counterparties;
	}

	public static FinanceAppContext LoadTransactions(this FinanceAppContext context)
	{
		if (context.Transactions.Any()) return context;

		Account[] accounts = context.LoadAccounts();
		Counterparty[] counterparties = context.LoadCounterparties();
		context.Transactions.AddRange(DatabaseSeeder.GetTransactions(accounts, counterparties));
		context.SaveChanges();
		return context;
	}
}