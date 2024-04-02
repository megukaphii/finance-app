using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators.Counterparty;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators.Counterparty;

[TestFixture]
[TestOf(typeof(CounterpartyFieldsValidator))]
public class CounterpartyFieldsValidatorTest
{
	[SetUp]
	public void SetUp()
	{
		_counterpartyFieldsValidator = new();
	}

	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int SafeNameLength = (MinNameLength + MaxNameLength) / 2;

	private CounterpartyFieldsValidator _counterpartyFieldsValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenNameIsOfValidLength()
	{
		ICounterpartyFields request = Substitute.For<ICounterpartyFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', SafeNameLength) });

		bool result = await _counterpartyFieldsValidator.ValidateAsync(request);

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

		ICounterpartyFields request = Substitute.For<ICounterpartyFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MinNameLength - 1) });

		bool result = await _counterpartyFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenNameIsTooLong()
	{
		ICounterpartyFields request = Substitute.For<ICounterpartyFields>();
		request.Name.Returns(new RequestField<string> { Value = new('a', MaxNameLength + 1) });

		bool result = await _counterpartyFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Name.Error, Is.Not.Empty);
	}
}