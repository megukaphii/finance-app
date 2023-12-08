using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Validators;

public class AccountIdValidator : IValidator<IAccountId>
{
	public Task<bool> ValidateAsync(IAccountId request)
	{
		FinanceAppContext db = new();
		return db.Accounts.AnyAsync(transaction => transaction.Id == request.Id.Value);
	}
}