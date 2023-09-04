using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class TransactionValidator : IValidator
{
    public bool Validate(IRequest request)
    {
        bool failure = false;
        if (request is ISingleTransaction validateAgainst) {
            if (validateAgainst.Value.Value < -1000) {
                validateAgainst.Value.Error = "Value should be greater than -1000";
                failure = true;
            }

            if (validateAgainst.Value.Value > 1000) {
                validateAgainst.Value.Error = "Value should be less than 1000";
                failure = true;
            }

            if (validateAgainst.Counterparty.Value.Name.Length < 255) {
                validateAgainst.Counterparty.Error = "Counterparty name length should be less than 255 characters";
                failure = true;
            }
        }

        return failure;
    }
}