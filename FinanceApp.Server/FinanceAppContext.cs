﻿using FinanceApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server;

public sealed class FinanceAppContext : DbContext
{
	public FinanceAppContext()
	{
		DbContextOptionsBuilder<FinanceAppContext> optionsBuilder = new();
		OnConfiguring(optionsBuilder);
	}

	public FinanceAppContext(DbContextOptions<FinanceAppContext> options)
		: base(options) { }

	public DbSet<Account> Accounts { get; set; } = null!;
	public DbSet<Counterparty> Counterparties { get; set; } = null!;
	public DbSet<Subscription> Subscriptions { get; set; } = null!;
	public DbSet<Transaction> Transactions { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		if (!options.IsConfigured) {
			options.UseSqlite(Directory.Exists("/Database") ? "Data Source=/Database/Finance.db" : "Data Source=test.db");
		}
	}
}