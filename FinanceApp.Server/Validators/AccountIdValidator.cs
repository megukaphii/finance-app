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

	public IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(IAccountId request)
	{
		return await UnitOfWork.Repository<Account>().AnyAsync(account => account.Id == request.Id.Value);
	}
}