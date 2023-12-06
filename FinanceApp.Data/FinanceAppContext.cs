using FinanceApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class FinanceAppContext : DbContext
{
	private const string DbPath = @"test.db";

	// TODO - This: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
	public DbSet<Account> Accounts { get; set; }
	public DbSet<Transaction> Transactions { get; set; }
	public DbSet<Counterparty> Counterparties { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder options) =>
		options.UseSqlite($"Data Source={DbPath}");
}