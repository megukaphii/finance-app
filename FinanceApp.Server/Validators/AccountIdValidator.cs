using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators;

public class AccountIdValidator : IValidator<IAccountId>
{
	public AccountIdValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(IAccountId request)
	{
		bool failure = false;
		if (!await UnitOfWork.Repository<Account>().AnyAsync(account => account.Id == request.Id.Value)) {
			request.Id.Error = $"Account with {nameof(request.Id)} of {request.Id} does not exist";
			failure = true;
		}

		return !failure;
	}
}