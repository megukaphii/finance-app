﻿using System.Diagnostics.CodeAnalysis;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Transaction;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators.Transaction;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators.Transaction;

[TestFixture]
[TestOf(typeof(TransactionFieldsValidator))]
public class TransactionFieldsValidatorTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadCounterparties();

		UnitOfWork unitOfWork = new(context);
		_transactionFieldsValidator = new(unitOfWork);
	}

	private TransactionFieldsValidator _transactionFieldsValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenCounterpartyExists()
	{
		ITransactionFields request = Substitute.For<ITransactionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = 1L });

		bool result = await _transactionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Counterparty.Error, Is.Empty);
	}

	[Test]
	[ExcludeFromCodeCoverage]
	public async Task ValidateAsync_ShouldReturnFalse_WhenCounterpartyDoesntExist()
	{
		ITransactionFields request = Substitute.For<ITransactionFields>();
		request.Counterparty.Returns(new RequestField<long> { Value = -1L });

		bool result = await _transactionFieldsValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Counterparty.Error, Is.Not.Empty);
	}
}