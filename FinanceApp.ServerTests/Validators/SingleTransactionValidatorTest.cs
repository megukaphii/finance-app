using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(SingleTransactionValidator))]
public class SingleTransactionValidatorTests
{
	[SetUp]
	public void SetUp()
	{
		_singleTransactionValidator = new();
	}

	private static readonly int MinCounterpartyNameLength = PropertyHelpers.GetMinLength((Counterparty c) => c.Name);
	private static readonly int MaxCounterpartyNameLength = PropertyHelpers.GetMaxLength((Counterparty c) => c.Name);
	private static readonly int SafeCounterpartyNameLength =
		(MinCounterpartyNameLength + MaxCounterpartyNameLength) / 2;

	private SingleTransactionValidator _singleTransactionValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenValueAndCounterpartyNameAreValid()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = 10M });
		request.Counterparty.Returns(new RequestField<Counterparty>
			{ Value = new() { Name = new('a', SafeCounterpartyNameLength) } });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Value.Error, Is.Empty);
		Assert.That(request.Counterparty.Error, Is.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenValueIsTooLow()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = decimal.MinValue });
		request.Counterparty.Returns(new RequestField<Counterparty>
			{ Value = new() { Name = new('a', SafeCounterpartyNameLength) } });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Value.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenValueIsTooHigh()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = decimal.MaxValue });
		request.Counterparty.Returns(new RequestField<Counterparty>
			{ Value = new() { Name = new('a', SafeCounterpartyNameLength) } });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Value.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyNameIsTooShort()
	{
		if (MinCounterpartyNameLength == 0) {
			Assert.Pass($"{nameof(MinCounterpartyNameLength)} is 0, validation cannot be failed");
		}

		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = 10M });
		request.Counterparty.Returns(new RequestField<Counterparty>
			{ Value = new() { Name = new('a', MinCounterpartyNameLength - 1) } });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Counterparty.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyNameIsTooLong()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = 10M });
		request.Counterparty.Returns(new RequestField<Counterparty>
			{ Value = new() { Name = new('a', MaxCounterpartyNameLength + 1) } });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Counterparty.Error, Is.Not.Empty);
	}
}