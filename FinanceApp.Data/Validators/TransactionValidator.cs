using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class TransactionValidator : IValidator
{
    public bool Validate(IRequest request)
    {
        if (request is ISingleTransaction validateAgainst) {
            if (validateAgainst.Value is < -1000 or > 1000) {
                return false;
            }

            if (validateAgainst.Counterparty.Name.Length < 255) {
                return false;
            }

            return true;
        }

        return false;
    }
}