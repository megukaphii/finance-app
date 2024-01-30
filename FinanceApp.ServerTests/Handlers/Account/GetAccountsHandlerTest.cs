using FinanceApp.Data;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Account;

[TestFixture]
[TestOf(typeof(GetAccountsHandler))]
public class GetAccountsHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = InMemoryDatabaseFactory.CreateNewDatabase();
		context.LoadAccounts();

		_mockClient = Substitute.For<IClient>();

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private IClient _mockClient = null!;
	private GetAccountsHandler _handler = null!;

	[TestCase(1)]
	[TestCase(10)]
	[TestCase(long.MaxValue)]
	public async Task Test_HandleAsync(long page)
	{
		List<Data.Models.Account> expectedAccounts = DatabaseSeeder.Accounts.OrderBy(account => account.Id).ToList();
		GetAccounts request = new()
		{
			Page = new() { Value = page }
		};

		await _handler.HandleAsync(request, _mockClient);

		// TODO - Update when we add actual pagination
		await _mockClient.Received().Send(Arg.Is<GetAccountsResponse>(r =>
			r.Success && r.Accounts.OrderBy(account => account.Id).SequenceEqual(expectedAccounts)));
	}
}