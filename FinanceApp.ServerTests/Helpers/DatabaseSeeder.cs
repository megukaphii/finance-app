﻿using FinanceApp.Data.Models;

namespace FinanceApp.ServerTests.Helpers;

public static class DatabaseSeeder
{
	public static Account[] Accounts => new[]
	{
		new Account
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
	};

	public static Counterparty[] Counterparties => new[]
	{
		new Counterparty
		{
			Name = "Counterparty 1"
		},
		new()
		{
			Name = "Counterparty 2"
		}
	};

	public static Transaction[] GetTransactions(Account[] accounts, Counterparty[] counterparties)
	{
		return new[]
		{
			new Transaction
			{
				Account = accounts[0],
				Counterparty = counterparties[0],
				Value = 200
			},
			new()
			{
				Account = accounts[0],
				Counterparty = counterparties[1],
				Value = 100
			},
			new()
			{
				Account = accounts[1],
				Counterparty = counterparties[1],
				Value = 100
			}
		};
	}
}