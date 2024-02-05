using FinanceApp.Data.RequestPatterns.Account;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators.Account;

public class AccountIdValidator : IValidator<IAccountId>
{
	public AccountIdValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(IAccountId request)
	{
		bool success = true;

		if (!await UnitOfWork.Repository<Data.Models.Account>().AnyAsync(account => account.Id == request.Id.Value)) {
			request.Id.Error = $"Account with {nameof(request.Id)} of {request.Id} does not exist";
			success = false;
		}

		return success;
	}
}