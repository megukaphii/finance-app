using FinanceApp.Server;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.ServerTests.Helpers;

public class InMemoryDatabaseFactory
{
	private string _currentGuid = string.Empty;

	public FinanceAppContext CreateNewDatabase()
	{
		_currentGuid = Guid.NewGuid().ToString();
		return GetExistingDatabase();
	}

	public FinanceAppContext GetExistingDatabase()
	{
		DbContextOptions<FinanceAppContext> options = new DbContextOptionsBuilder<FinanceAppContext>()
			.UseInMemoryDatabase(_currentGuid)
			.Options;
		FinanceAppContext context = new(options);
		return context;
	}
}