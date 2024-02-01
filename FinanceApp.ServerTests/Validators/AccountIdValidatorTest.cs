using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Validators;

[TestFixture]
[TestOf(typeof(AccountIdValidator))]
public class AccountIdValidatorTests
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = new InMemoryDatabaseFactory().CreateNewDatabase();
		context.LoadAccounts();

		UnitOfWork unitOfWork = new(context);
		_accountIdValidator = new(unitOfWork);
	}

	private AccountIdValidator _accountIdValidator = null!;

	[Test]
	public async Task ValidateAsync_ShouldReturnTrue_WhenAccountExists()
	{
		IAccountId request = Substitute.For<IAccountId>();
		request.Id.Returns(new RequestField<long> { Value = 1 });

		bool result = await _accountIdValidator.ValidateAsync(request);

		Assert.That(result, Is.True);
		Assert.That(request.Id.Error, Is.Empty);
	}

	[Test]
	public async Task ValidateAsync_ShouldReturnFalse_WhenAccountDoesNotExist()
	{
		IAccountId request = Substitute.For<IAccountId>();
		request.Id.Returns(new RequestField<long> { Value = 99 });

		bool result = await _accountIdValidator.ValidateAsync(request);

		Assert.That(result, Is.False);
		Assert.That(request.Id.Error, Is.Not.Empty);
	}
}