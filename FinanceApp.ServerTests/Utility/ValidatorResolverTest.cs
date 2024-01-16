using System.Reflection;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using IRequest = FinanceApp.Data.Interfaces.IRequest;

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

	private static IEnumerable<Type[]> ValidatorTypes
	{
		get
		{
			yield return [typeof(IAccountId), typeof(SelectAccount)];
			yield return [typeof(IPageNumber), typeof(GetAccounts)];
			yield return [typeof(ISingleAccount), typeof(CreateAccount)];
			yield return [typeof(ISingleTransaction), typeof(CreateTransaction)];
		}
	}

	[TestCaseSource(nameof(ValidatorTypes))]
	public void GetValidator_T_ReturnsCorrectValidator(Type validatorType, Type requestType)
	{
		MethodInfo? method = GetType().GetMethod(nameof(GetValidatorHelper));
		MethodInfo? genericMethod = method?.MakeGenericMethod(validatorType, requestType);
		genericMethod?.Invoke(this, null);
	}

	public void GetValidatorHelper<TValidator, TRequest>() where TValidator : IRequest where TRequest : IRequest
	{
		IValidator<TValidator>? validator = Substitute.For<IValidator<TValidator>>();
		_serviceProvider.GetService(typeof(IValidator<TValidator>)).Returns(validator);

		dynamic returnedValidator = _validatorResolver.GetValidator<TRequest>();

		Assert.That(validator, Is.EqualTo(returnedValidator));
	}

	[Test]
	public void GetValidator_T_ThrowsExceptionWhenNoValidator()
	{
		_serviceProvider.GetService(typeof(IValidator<IAccountId>)).Returns(null);

		Assert.Throws<InvalidOperationException>(() => _validatorResolver.GetValidator<SelectAccount>());
	}
}