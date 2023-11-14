using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class AccountIdValidator : IValidator
{
    public bool Validate(IRequest request)
    {
        bool failure = false;
        if (request is IAccountId validateAgainst) {
            // TODO - Needs to check if ID exists in DB
        }

        return !failure;
    }
}