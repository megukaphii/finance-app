using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators.Counterparty;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators.Counterparty;

[TestFixture]
[TestOf(typeof(CounterpartyFullValidator))]
public class CounterpartyFullValidatorTest
{
	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;

	private CounterpartyFullValidator _counterpartyFullValidator = null!;

	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadCounterparties();

		UnitOfWork unitOfWork = new(context);
		_counterpartyFullValidator = new(unitOfWork);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenCounterpartyExists()
	{
		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Id.Error, Is.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesNotExist()
	{
		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameIsWithinValidLengths()
	{
		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Name.Error, Is.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooShort()
	{
		if (MinNameLength == 0) {
			Assert.Pass($"{nameof(MinNameLength)} is 0, validation cannot be failed");
		}

		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesNotExistAndNameIsTooShort()
	{
		if (MinNameLength == 0) {
			Assert.Pass($"{nameof(MinNameLength)} is 0, validation cannot be failed");
		}

		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesNotExistAndNameIsTooLong()
	{
		ICounterpartyFull request = Substitute.For<ICounterpartyFull>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });

		bool result = await _counterpartyFullValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}
}