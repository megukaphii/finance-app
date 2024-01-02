using FinanceApp.Data;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Validators;

public class AccountIdValidator : IValidator<IAccountId>
{
	public Task<bool> ValidateAsync(IAccountId request)
	{
		FinanceAppContext db = new();
		return db.Accounts.AnyAsync(transaction => transaction.Id == request.Id.Value);
	}
}