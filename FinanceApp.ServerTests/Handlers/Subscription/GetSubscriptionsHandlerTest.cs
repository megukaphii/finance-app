using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Subscription;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Subscription;

[TestFixture]
[TestOf(typeof(GetSubscriptionsHandler))]
public class GetSubscriptionsHandlerTest
{
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		_session = Substitute.For<ISession>();
	}

	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadSubscriptions();

		_client = Substitute.For<IClient>();
		_client.Session.Returns(_session);
		_session.AccountId.Returns(1L);
		_session.IsAccountSet().Returns(true);

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private ISession _session = null!;
	private IClient _client = null!;
	private GetSubscriptionsHandler _handler = null!;

	[Test]
	public async Task HandleAsync_SendsCorrectResponse()
	{
		List<Data.Models.Subscription> expectedSubscriptions = DatabaseSeeder
			.GetSubscriptions(DatabaseSeeder.Accounts, DatabaseSeeder.Counterparties)
			.Where(subscription => subscription.Account.Equals(DatabaseSeeder.Accounts[0]))
			.OrderBy(subscription => subscription.Id).ToList();
		GetSubscriptions request = new() { Page = new() { Value = 1 } };

		await _handler.HandleAsync(request, _client);

		await _client.Received().Send(Arg.Is<GetSubscriptionsResponse>(response =>
			response.Success && response.Subscriptions.OrderBy(subscription => subscription.Id)
				.SequenceEqual(expectedSubscriptions)));
	}

	[Test]
	public async Task HandleAsync_WithEmptyAccount_ShouldNotSendSubscriptions()
	{
		_session.IsAccountSet().Returns(false);
		GetSubscriptions request = new() { Page = new() { Value = 1 } };

		await _handler.HandleAsync(request, _client);

		await _client.Received().Send(Arg.Is<GetSubscriptionsResponse>(response => !response.Success && response.Subscriptions.Count == 0));
	}
}