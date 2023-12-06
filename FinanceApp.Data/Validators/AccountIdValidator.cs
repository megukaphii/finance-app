using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Validators;

public class AccountIdValidator : IValidator
{
	public async Task<bool> ValidateAsync(IRequest request)
	{
		bool pass = false;
		if (request is IAccountId validateAgainst) {
			FinanceAppContext db = new();
			pass = await db.Accounts.AnyAsync(transaction => transaction.Id == validateAgainst.Id.Value);
		}

		return pass;
	}
}