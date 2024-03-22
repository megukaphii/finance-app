using FinanceApp.Data.Enums;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Subscription;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Subscription;

[TestFixture]
[TestOf(typeof(CreateSubscriptionHandler))]
public class CreateSubscriptionHandlerTest
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
	private CreateSubscriptionHandler _handler = null!;

	[Test]
	public async Task CreateSubscriptionHandler_HandleAsync_CreatesSubscriptionSuccessfully()
	{
		CreateSubscription request = new()
		{
			Counterparty = new() { Value = 1L },
			Name = new() { Value = "Test" },
			Value = new() { Value = 123.45m },
			FrequencyCounter = new() { Value = 1 },
			FrequencyMeasure = new() { Value = Frequency.Weekly },
			StartDate = new() { Value = DateTime.Today },
			EndDate = new() { Value = DateTime.UnixEpoch }
		};
		Data.Models.Subscription expected = new()
		{
			Account = DatabaseSeeder.Accounts[0],
			Counterparty = DatabaseSeeder.Counterparties[0],
			Name = "Test",
			Value = 123.45m,
			FrequencyCounter = 1,
			FrequencyMeasure = Frequency.Weekly,
			StartDate = DateTime.Today,
			EndDate = DateTime.UnixEpoch
		};

		await _handler.HandleAsync(request, _client);
		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		Data.Models.Subscription actual = await unitOfWork.Repository<Data.Models.Subscription>()
			                                  .IncludeAll(subscription => subscription.Account,
				                                  transaction => transaction.Counterparty)
			                                  .FirstAsync(subscription => subscription.Id == 1L);

		Assert.That(actual, Is.EqualTo(expected));
		await _client.Received().Send(Arg.Is<CreateSubscriptionResponse>(r => r.Success && r.Id > 0));
	}

	[Test]
	public async Task CreateSubscriptionHandler_HandleAsync_WhenAccountNotSet_DoesNotCreateSubscription()
	{
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new(unitOfWork);
		_client.Session.IsAccountSet().Returns(false);
		CreateSubscription request = new()
		{
			Counterparty = new() { Value = 1L },
			Name = new() { Value = "Test" },
			Value = new() { Value = 123.45m },
			FrequencyCounter = new() { Value = 1 },
			FrequencyMeasure = new() { Value = Frequency.Weekly },
			StartDate = new() { Value = DateTime.Today },
			EndDate = new() { Value = DateTime.UnixEpoch }
		};

		await _handler.HandleAsync(request, _client);

		await unitOfWork.Repository<Data.Models.Subscription>().DidNotReceive().AddAsync(Arg.Any<Data.Models.Subscription>());
		await _client.Received().Send(Arg.Is<CreateSubscriptionResponse>(r => !r.Success && r.Id == 0));
	}
}