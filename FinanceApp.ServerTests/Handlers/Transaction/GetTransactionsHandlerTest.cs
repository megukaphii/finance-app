using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Transaction;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Transaction;

[TestFixture]
[TestOf(typeof(GetTransactionsHandler))]
public class GetTransactionsHandlerTest
{
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		_mockSession = Substitute.For<ISession>();
	}

	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadTransactions();

		_mockClient = Substitute.For<IClient>();
		_mockClient.Session.Returns(_mockSession);
		_mockSession.Account.Returns(context.Accounts.Find(1L)!);

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private ISession _mockSession = null!;
	private IClient _mockClient = null!;
	private GetTransactionsHandler _handler = null!;

	[Test]
	public async Task HandleAsync_SendsCorrectResponse()
	{
		List<Data.Models.Transaction> expectedTransactions = DatabaseSeeder
			.GetTransactions(DatabaseSeeder.Accounts, DatabaseSeeder.Counterparties)
			.Where(transaction => transaction.Account.Equals(DatabaseSeeder.Accounts[0]))
			.OrderBy(transaction => transaction.Id).ToList();
		GetTransactions request = new() { Page = new() { Value = 1 } };

		await _handler.HandleAsync(request, _mockClient);

		await _mockClient.Received().Send(Arg.Is<GetTransactionsResponse>(response =>
			response.Success && response.Transactions.OrderBy(transaction => transaction.Id)
				.SequenceEqual(expectedTransactions)));
	}

	[Test]
	public async Task HandleAsync_WithEmptyAccount_ShouldNotSendTransactions()
	{
		_mockSession.Account = new()
		{
			Name = "Empty Account",
			Description = "Empty Account",
			Value = 0
		};
		GetTransactions request = new() { Page = new() { Value = 1 } };

		await _handler.HandleAsync(request, _mockClient);

		await _mockClient.Received().Send(Arg.Is<GetTransactionsResponse>(response =>
			response.Success && response.Transactions.Count == 0));
	}
}