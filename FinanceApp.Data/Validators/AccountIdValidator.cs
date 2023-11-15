using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class AccountIdValidator : IValidator
{
    public bool Validate(IRequest request, FinanceAppContext db)
    {
        bool pass = false;
        if (request is IAccountId validateAgainst) {
            pass = db.Accounts.Any(transaction => transaction.Id == validateAgainst.Id.Value);
        }

        return pass;
    }
}