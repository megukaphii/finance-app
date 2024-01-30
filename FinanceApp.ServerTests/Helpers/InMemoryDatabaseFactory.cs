using FinanceApp.Data;
using FinanceApp.Server;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.ServerTests.Helpers;

public static class InMemoryDatabaseFactory
{
	public static FinanceAppContext CreateNewDatabase()
	{
		DbContextOptions<FinanceAppContext> options = new DbContextOptionsBuilder<FinanceAppContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		FinanceAppContext context = new(options);
		return context;
	}
}