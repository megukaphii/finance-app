using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;

namespace FinanceApp.ServerTests.Utility;

[TestFixture]
[TestOf(typeof(ValidatorResolver))]
public class ValidatorResolverTests
{
	[SetUp]
	public void SetUp()
	{
		_serviceProvider = Substitute.For<IServiceProvider>();
		_validatorResolver = new(_serviceProvider);
	}

	private ValidatorResolver _validatorResolver = null!;
	private IServiceProvider _serviceProvider = null!;

	[Test]
	public void GetValidator_T_ReturnsCorrectValidator()
	{
		IValidator<IAccountId>? accountIdValidator = Substitute.For<IValidator<IAccountId>>();
		_serviceProvider.GetService(typeof(IValidator<IAccountId>)).Returns(accountIdValidator);

		IValidator<IAccountId> validator = _validatorResolver.GetValidator<IAccountId>();

		Assert.That(accountIdValidator, Is.EqualTo(validator));
	}

	[Test]
	public void GetValidator_T_ThrowsExceptionWhenNoValidator()
	{
		_serviceProvider.GetService(typeof(IValidator<IAccountId>)).Returns(null);

		Assert.Throws<InvalidOperationException>(() => _validatorResolver.GetValidator<IAccountId>());
	}
}