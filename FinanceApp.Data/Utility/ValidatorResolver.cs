using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Data.Requests.Transaction;

namespace FinanceApp.Data.Utility;

public class ValidatorResolver : IValidatorResolver
{
	private readonly Dictionary<Type, object> _validators;

	public ValidatorResolver(IServiceProvider serviceProvider)
	{
		_validators = new()
		{
			{ typeof(CreateAccount), serviceProvider.GetValidator<ISingleAccount>() },
			{ typeof(GetAccounts), serviceProvider.GetValidator<IPageNumber>() },
			{ typeof(SelectAccount), serviceProvider.GetValidator<IAccountId>() },
			{ typeof(GetCounterparties), serviceProvider.GetValidator<IPageNumber>() },
			{ typeof(CreateTransaction), serviceProvider.GetValidator<ISingleTransaction>() },
			{ typeof(GetTransactions), serviceProvider.GetValidator<IPageNumber>() }
		};
	}

	public IValidator<T> GetValidator<T>() where T : IRequest
	{
		// TODO - Use reflection to get the appropriate request pattern, then just resolve the IValidator from there
		object validator = _validators[typeof(T)];
		return (IValidator<T>)validator;
	}
}