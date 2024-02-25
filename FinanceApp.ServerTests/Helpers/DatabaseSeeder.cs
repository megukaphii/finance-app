using FinanceApp.Data.Enums;
using FinanceApp.Data.Models;

namespace FinanceApp.ServerTests.Helpers;

public static class DatabaseSeeder
{
	public static Account[] Accounts =>
	[
		new()
		{
			Name = "Account 1",
			Description = "Account 1",
			Value = 0
		},
		new()
		{
			Name = "Account 2",
			Description = "Account 2",
			Value = 0
		}
	];

	public static Counterparty[] Counterparties =>
	[
		new()
		{
			Name = "Counterparty 1"
		},
		new()
		{
			Name = "Counterparty 2"
		}
	];

	public static Transaction[] GetTransactions(Account[] accounts, Counterparty[] counterparties)
	{
		return
		[
			new()
			{
				Account = accounts[0],
				Counterparty = counterparties[0],
				Value = 200,
				Timestamp = default
			},
			new()
			{
				Account = accounts[0],
				Counterparty = counterparties[1],
				Value = 100,
				Timestamp = DateTime.Now + TimeSpan.FromDays(1)
			},
			new()
			{
				Account = accounts[1],
				Counterparty = counterparties[1],
				Value = 100,
				Timestamp = DateTime.Now - TimeSpan.FromDays(1)
			}
		];
	}

	public static Subscription[] GetSubscriptions(Account[] accounts, Counterparty[] counterparties) =>
	[
		new()
		{
			Account = accounts[0],
			Counterparty = counterparties[0],
			Name = "Subscription 1",
			Value = 12.99m,
			FrequencyCounter = 1,
			FrequencyMeasure = Frequency.Monthly,
			StartDate = DateTime.Today,
			EndDate = DateTime.UnixEpoch
		},
		new()
		{
			Account = accounts[0],
			Counterparty = counterparties[1],
			Name = "Subscription 2",
			Value = 18.99m,
			FrequencyCounter = 1,
			FrequencyMeasure = Frequency.Yearly,
			StartDate = DateTime.Today,
			EndDate = DateTime.Today + TimeSpan.FromDays(365 * 5 + 30)
		},
		new()
		{
			Account = accounts[1],
			Counterparty = counterparties[1],
			Name = "Subscription 3",
			Value = 5.99m,
			FrequencyCounter = 2,
			FrequencyMeasure = Frequency.Weekly,
			StartDate = DateTime.Today,
			EndDate = DateTime.UnixEpoch
		}
	];
}