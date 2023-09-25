using FinanceApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class FinanceAppContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Counterparty> Counterparties { get; set; }

    private const string DbPath = @"test.db";

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}