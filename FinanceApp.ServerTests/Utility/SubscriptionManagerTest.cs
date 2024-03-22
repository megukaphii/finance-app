using FinanceApp.Data.Models;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.ServerTests.Utility;

[TestFixture]
[TestOf(typeof(SubscriptionManager))]
public class SubscriptionManagerTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = _databaseFactory.CreateNewDatabase();
		context.LoadSubscriptions();

		UnitOfWork unitOfWork = new(context);
		_subscriptionManager = new(unitOfWork);
	}

	private readonly InMemoryDatabaseFactory _databaseFactory = new();
	private SubscriptionManager _subscriptionManager = null!;

	[Test]
	public async Task ApplyDueSubscriptions_ShouldApplySubscriptionsSuccessfully()
	{
		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		List<Subscription> subscriptions = await unitOfWork.Repository<Subscription>()
			                                   .IncludeAll(subscription => subscription.Account, subscription => subscription.Counterparty)
			                                   .OrderBy(subscription => subscription.Id).ToListAsync();
		List<Transaction> expected =
		[
			new()
			{
				Subscription = subscriptions[0],
				Account = subscriptions[0].Account,
				Counterparty = subscriptions[0].Counterparty,
				Value = subscriptions[0].Value,
				Timestamp = DateTime.Today
			},
			new()
			{
				Subscription = subscriptions[1],
				Account = subscriptions[1].Account,
				Counterparty = subscriptions[1].Counterparty,
				Value = subscriptions[1].Value,
				Timestamp = DateTime.Today
			},
			new()
			{
				Subscription = subscriptions[3],
				Account = subscriptions[3].Account,
				Counterparty = subscriptions[3].Counterparty,
				Value = subscriptions[3].Value,
				Timestamp = DateTime.Today
			},
			new()
			{
				Subscription = subscriptions[4],
				Account = subscriptions[4].Account,
				Counterparty = subscriptions[4].Counterparty,
				Value = subscriptions[4].Value,
				Timestamp = DateTime.Today
			}
		];
		expected[0].Account.Value = 132.98m;
		expected[2].Account.Value = 184.98m;

		await _subscriptionManager.ApplyDueSubscriptions();
		context = _databaseFactory.GetExistingDatabase();
		unitOfWork = new(context);
		List<Transaction> result = await unitOfWork.Repository<Transaction>()
			                           .IncludeAll(transaction => transaction.Subscription!, transaction => transaction.Account,
				                           transaction => transaction.Counterparty).OrderBy(transaction => transaction.Subscription!.Id)
			                           .ToListAsync();
		decimal account1Value = (await unitOfWork.Repository<Account>().FindAsync(1L))!.Value;
		decimal account2Value = (await unitOfWork.Repository<Account>().FindAsync(2L))!.Value;

		Assert.That(result, Is.EqualTo(expected));
		Assert.That(account1Value, Is.EqualTo(subscriptions[0].Value + subscriptions[1].Value));
		Assert.That(account2Value, Is.EqualTo(subscriptions[3].Value + subscriptions[4].Value));
	}

	[Test]
	public async Task GetActiveSubscriptions_ShouldReturnActiveSubscriptions()
	{
		FinanceAppContext db = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(db);
		List<Subscription> subscriptions = await unitOfWork.Repository<Subscription>().Include(subscription => subscription.Account)
			                                   .Include(subscription => subscription.Counterparty).ToListAsync();
		List<Subscription> expected = [subscriptions[0], subscriptions[1], subscriptions[3], subscriptions[4], subscriptions[5]];

		List<Subscription> result = await _subscriptionManager.GetActiveSubscriptions().ToListAsync();

		Assert.That(result, Is.EqualTo(expected));
	}
}