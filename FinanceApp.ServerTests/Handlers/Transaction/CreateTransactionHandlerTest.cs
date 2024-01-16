using FinanceApp.Data;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Handlers.Transaction;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Transaction;

[TestFixture]
[TestOf(typeof(CreateTransactionHandler))]
public class CreateTransactionHandlerTest
{
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		_mockSession = Substitute.For<ISession>();
	}

	[SetUp]
	public void SetUp()
	{
		_context = InMemoryDatabaseFactory.CreateNewDatabase();
		_context.LoadAccounts();

		_mockClient = Substitute.For<IClient>();
		_mockClient.Session.Returns(_mockSession);
		_mockSession.Account.Returns(_context.Accounts.Find(1L)!);
		_mockSession.IsAccountSet().Returns(true);

		_mockUnitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new(_mockUnitOfWork);
	}

	private ISession _mockSession = null!;
	private IClient _mockClient = null!;
	private FinanceAppContext _context = null!;
	private IUnitOfWork _mockUnitOfWork = null!;
	private CreateTransactionHandler _handler = null!;

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_CreatesTransactionSuccessfully()
	{
		Data.Models.Transaction addedTransaction = null!;
		_mockUnitOfWork.Repository<Data.Models.Transaction>()
			.When(x => x.AddAsync(Arg.Any<Data.Models.Transaction>()))
			.Do(c =>
			{
				addedTransaction = c.Arg<Data.Models.Transaction>();
				UnitOfWork unitOfWork = new(_context);
				unitOfWork.Repository<Data.Models.Transaction>().AddAsync(addedTransaction);
				unitOfWork.SaveChanges();
			});
		Data.Models.Transaction expectedTransaction = new()
			{ Account = _mockSession.Account, Counterparty = new() { Name = "Test Party" }, Value = 123.45m };
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = new() { Id = 1, Name = "Test Party" } },
			Value = new() { Value = 123.45m }
		};

		await _handler.HandleAsync(request, _mockClient);

		Assert.That(addedTransaction, Is.EqualTo(expectedTransaction));
		_mockUnitOfWork.Received().SaveChanges();
		await _mockClient.Received().Send(Arg.Is<CreateTransactionResponse>(r => r.Success && r.Id > 0));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_WhenAccountNotSet_DoesNotCreateTransaction()
	{
		_mockSession.IsAccountSet().Returns(false);
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = new() { Id = 1, Name = "Test Party" } },
			Value = new() { Value = 123.45m }
		};

		await _handler.HandleAsync(request, _mockClient);

		await _mockUnitOfWork.Repository<Data.Models.Transaction>().DidNotReceive()
			.AddAsync(Arg.Any<Data.Models.Transaction>());
		await _mockClient.Received().Send(Arg.Is<CreateTransactionResponse>(r => !r.Success && r.Id == 0));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_AddsNewCounterpartyWhenIdIsZero()
	{
		Data.Models.Counterparty expectedCounterparty = new() { Name = "New Party" };
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = expectedCounterparty },
			Value = new() { Value = 123.45m }
		};

		await _handler.HandleAsync(request, _mockClient);

		await _mockUnitOfWork.Repository<Data.Models.Counterparty>().Received()
			.AddAsync(Arg.Is<Data.Models.Counterparty>(c => c.Equals(expectedCounterparty)));
		_mockUnitOfWork.Received().SaveChanges();
		await _mockClient.Received().Send(Arg.Is<CreateTransactionResponse>(r => r.Success));
	}

	[Test]
	public async Task CreateTransactionHandler_HandleAsync_DoesNotAddNewCounterpartyWhenIdIsNotZero()
	{
		CreateTransaction request = new()
		{
			Counterparty = new() { Value = new() { Id = 1, Name = "New Party" } },
			Value = new() { Value = 123.45m }
		};

		await _handler.HandleAsync(request, _mockClient);

		await _mockUnitOfWork.Repository<Data.Models.Counterparty>().DidNotReceive()
			.AddAsync(Arg.Any<Data.Models.Counterparty>());
	}
}