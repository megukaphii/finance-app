using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Subscription;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators.Subscription;
using FinanceApp.Server.Validators.Transaction;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators.Subscription;

[TestFixture]
[TestOf(typeof(TransactionFieldsValidator))]
public class SubscriptionFieldsValidatorTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadCounterparties();

		UnitOfWork unitOfWork = new(context);
		_subscriptionFieldsValidator = new(unitOfWork);
	}

	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Subscription a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Subscription a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;
	private static readonly int MinFrequencyCounterValue =
		PropertyHelpers.GetMinValue((Data.Models.Subscription a) => a.FrequencyCounter);
	private static readonly int MaxFrequencyCounterValue =
		PropertyHelpers.GetMaxValue((Data.Models.Subscription a) => a.FrequencyCounter);
	private static readonly int SafeFrequencyCounterValue = (MinFrequencyCounterValue + MaxFrequencyCounterValue) / 2;

	private SubscriptionFieldsValidator _subscriptionFieldsValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameAndFrequencyCounterAndCounterpartyAreValid()
	{
		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = SafeFrequencyCounterValue });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Counterparty.Error, Is.Empty);
		Assert.That(request.Name.Error, Is.Empty);
		Assert.That(request.FrequencyCounter.Error, Is.Empty);
	}

	// TODO - Turn into single test with array of requests passed in and asserted against
	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooShort()
	{
		if (MinNameLength == 0) Assert.Pass($"{nameof(MinNameLength)} is 0, validation cannot be failed");

		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = SafeFrequencyCounterValue });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = SafeFrequencyCounterValue });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenFrequencyCounterIsTooLow()
	{
		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = MinFrequencyCounterValue - 1 });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.FrequencyCounter.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenFrequencyCounterIsTooHigh()
	{
		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = MaxFrequencyCounterValue + 1 });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.FrequencyCounter.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesntExist()
	{
		ISubscriptionFields request = Substitute.For<ISubscriptionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = -1L });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });
		request.FrequencyCounter.Returns(new RequestField<int> { Value = SafeFrequencyCounterValue });

		bool result = await _subscriptionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Counterparty.Error, Is.Not.Empty);
	}
}