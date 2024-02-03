using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(SingleTransactionValidator))]
public class SingleTransactionValidatorTests
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadCounterparties();

		UnitOfWork unitOfWork = new(context);
		_singleTransactionValidator = new(unitOfWork);
	}

	private SingleTransactionValidator _singleTransactionValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenValueAndCounterpartyAreValid()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = 10M });
		request.Counterparty.Returns(new RequestField<long>{ Value = 1L });

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
		request.Counterparty.Returns(new RequestField<long>{ Value = 1L });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Value.Error, Is.Not.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenValueIsTooHigh()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = decimal.MaxValue });
		request.Counterparty.Returns(new RequestField<long>{ Value = 1L });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Value.Error, Is.Not.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesntExist()
	{
		ISingleTransaction request = Substitute.For<ISingleTransaction>();
		request.Value.Returns(new RequestField<decimal> { Value = 10M });
		request.Counterparty.Returns(new RequestField<long>{ Value = -1L });

		bool result = await _singleTransactionValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Counterparty.Error, Is.Not.Empty);
	}
}