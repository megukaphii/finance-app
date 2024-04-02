using FinanceApp.Data.Interfaces;
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

		_client = Substitute.For<IClient>();
		ISession session = Substitute.For<ISession>();
		session.AccountId.Returns(1L);
		session.IsAccountSet().Returns(true);
		_client.Session.Returns(session);

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private readonly InMemoryDatabaseFactory _databaseFactory = new();
	private IClient _client = null!;
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
			Account = DatabaseSeeder.Accounts[0],
			Counterparty = DatabaseSeeder.Counterparties[0],
			Value = 123.45m,
			Timestamp = default
		};
		expected.Account.Value = 123.45m;

		await _handler.HandleAsync(request, _client);
		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		Data.Models.Transaction actual = await unitOfWork.Repository<Data.Models.Transaction>()
			                                 .IncludeAll(transaction => transaction.Account, transaction => transaction.Counterparty)
			                                 .FirstAsync(transaction => transaction.Id == 1L);

		Assert.That(actual, Is.EqualTo(expected));
		await _client.Received().Send(Arg.Is<CreateTransactionResponse>(r => r.Success && r.Id > 0));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_MultipleTransactionsDoNotThrowException()
	{
		await using (FinanceAppContext context = _databaseFactory.GetExistingDatabase()) {
			{
				UnitOfWork unitOfWork = new(context);
				_handler = new(unitOfWork);
				CreateTransaction request1 = new()
				{
					Counterparty = new()
					{
						Value = 1L
					},
					Value = new() { Value = 123.45m },
					Timestamp = new() { Value = default }
				};

				await _handler.HandleAsync(request1, _client);
			}
		}

		Assert.DoesNotThrowAsync(async () =>
		{
			await using FinanceAppContext context = _databaseFactory.GetExistingDatabase();
			UnitOfWork unitOfWork = new(context);
			_handler = new(unitOfWork);
			CreateTransaction request2 = new()
			{
				Counterparty = new() { Value = 1L },
				Value = new() { Value = 123.45m },
				Timestamp = new() { Value = default }
			};

			await _handler.HandleAsync(request2, _client);
		});
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_TracksAccountValueCorrectly()
	{
		CreateTransaction request1 = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};
		CreateTransaction request2 = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 543.21m },
			Timestamp = new() { Value = default }
		};
		CreateTransaction request3 = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = -321.45m },
			Timestamp = new() { Value = default }
		};

		await _handler.HandleAsync(request1, _client);
		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
		await _handler.HandleAsync(request2, _client);
		context = _databaseFactory.GetExistingDatabase();
		unitOfWork = new(context);
		_handler = new(unitOfWork);
		await _handler.HandleAsync(request3, _client);

		context = _databaseFactory.GetExistingDatabase();
		unitOfWork = new(context);
		decimal result = (await unitOfWork.Repository<Data.Models.Account>().FindAsync(_client.Session.AccountId))!.Value;
		Assert.That(result, Is.EqualTo(request1.Value.Value + request2.Value.Value + request3.Value.Value));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_WhenAccountNotSet_DoesNotCreateTransaction()
	{
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new(unitOfWork);
		_client.Session.IsAccountSet().Returns(false);
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = 1L },
			Value = new() { Value = 123.45m },
			Timestamp = new() { Value = default }
		};

		await _handler.HandleAsync(request, _client);

		await unitOfWork.Repository<Data.Models.Transaction>().DidNotReceive().AddAsync(Arg.Any<Data.Models.Transaction>());
		await _client.Received().Send(Arg.Is<CreateTransactionResponse>(r => !r.Success && r.Id == 0));
	}
}