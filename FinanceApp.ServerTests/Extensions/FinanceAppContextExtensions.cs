using FinanceApp.Data.Models;
using FinanceApp.Server;
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

	public static Transaction[] LoadTransactions(this FinanceAppContext context)
	{
		if (context.Transactions.Any()) return context.Transactions.ToArray();

		Account[] accounts = context.LoadAccounts();
		Counterparty[] counterparties = context.LoadCounterparties();
		Transaction[] transactions = DatabaseSeeder.GetTransactions(accounts, counterparties);
		context.Transactions.AddRange(transactions);
		context.SaveChanges();
		return transactions;
	}

	public static Subscription[] LoadSubscriptions(this FinanceAppContext context)
	{
		if (context.Subscriptions.Any()) return context.Subscriptions.ToArray();

		Account[] accounts = context.LoadAccounts();
		Counterparty[] counterparties = context.LoadCounterparties();
		Subscription[] subscriptions = DatabaseSeeder.GetSubscriptions(accounts, counterparties);
		context.Subscriptions.AddRange(subscriptions);
		context.SaveChanges();
		return subscriptions;
	}
}