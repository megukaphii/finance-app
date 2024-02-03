using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Transaction;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Transaction;

[TestFixture]
[TestOf(typeof(CreateTransactionHandler))]
public class CreateTransactionHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = _databaseFactory.CreateNewDatabase();
		context.LoadAccounts();
		context.LoadCounterparties();

		_mockClient = Substitute.For<IClient>();
		ISession session = Substitute.For<ISession>();
		session.Account.Returns(context.Accounts.Find(1L)!);
		session.IsAccountSet().Returns(true);
		_mockClient.Session.Returns(session);

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private readonly InMemoryDatabaseFactory _databaseFactory = new();
	private IClient _mockClient = null!;
	private CreateTransactionHandler _handler = null!;

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_CreatesTransactionSuccessfully()
	{
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};
		Data.Models.Transaction expected = new()
		{
			Account = _mockClient.Session.Account,
			Counterparty = DatabaseSeeder.Counterparties[0],
			Value = 123.45m,
			Timestamp = default
		};

		await _handler.HandleAsync(request, _mockClient);
		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		Data.Models.Transaction? actual = await unitOfWork.Repository<Data.Models.Transaction>()
			                                  .IncludeAll(transaction => transaction.Account,
				                                  transaction => transaction.Counterparty)
			                                  .FirstAsync(transaction => transaction.Id == 1L);

		Assert.That(actual, Is.EqualTo(expected));
		await _mockClient.Received().Send(Arg.Is<CreateTransactionResponse>(r => r.Success && r.Id > 0));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_MultipleTransactionsDoNotThrowException()
	{
		InMemoryDatabaseFactory databaseFactory = new();
		FinanceAppContext context = databaseFactory.CreateNewDatabase();
		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
		CreateTransaction request1 = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};
		CreateTransaction request2 = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};

		await _handler.HandleAsync(request1, _mockClient);

		Assert.DoesNotThrowAsync(async () =>
		{
			context = databaseFactory.GetExistingDatabase();
			unitOfWork = new(context);
			_handler = new(unitOfWork);
			await _handler.HandleAsync(request2, _mockClient);
		});
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_WhenAccountNotSet_DoesNotCreateTransaction()
	{
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new(unitOfWork);
		_mockClient.Session.IsAccountSet().Returns(false);
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};

		await _handler.HandleAsync(request, _mockClient);

		await unitOfWork.Repository<Data.Models.Transaction>().DidNotReceive()
			.AddAsync(Arg.Any<Data.Models.Transaction>());
		await _mockClient.Received().Send(Arg.Is<CreateTransactionResponse>(r => !r.Success && r.Id == 0));
	}
}