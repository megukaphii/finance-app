using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(SingleCounterpartyValidator))]
public class SingleCounterpartyValidatorTests
{
	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Counterparty a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Counterparty a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;

	private SingleCounterpartyValidator _singleCounterpartyValidator = null!;

	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadCounterparties();

		UnitOfWork unitOfWork = new(context);
		_singleCounterpartyValidator = new(unitOfWork);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenCounterpartyExists()
	{
		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Id.Error, Is.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesNotExist()
	{
		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameIsWithinValidLengths()
	{
		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

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

		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 1 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

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

		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesNotExistAndNameIsTooLong()
	{
		ISingleCounterparty request = Substitute.For<ISingleCounterparty>();
		request.Id.Returns(new RequestField<long> { Value = 99 });
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });

		bool result = await _singleCounterpartyValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}
}