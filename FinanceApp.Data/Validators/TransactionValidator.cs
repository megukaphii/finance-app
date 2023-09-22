using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class TransactionValidator : IValidator
{
    private const double MinValue = double.MinValue;
    private const double MaxValue = double.MaxValue;
    // TODO - Get max length from Counterparty directly via reflection
    private const int MaxCounterpartyNameLength = 255;

    public bool Validate(IRequest request)
    {
        bool failure = false;
        if (request is ISingleTransaction validateAgainst) {
            if (validateAgainst.Value.Value < MinValue) {
                validateAgainst.Value.Error = $"{nameof(validateAgainst.Value)} should be greater than {MinValue}";
                failure = true;
            } else if (validateAgainst.Value.Value > MaxValue) {
                validateAgainst.Value.Error = $"{nameof(validateAgainst.Value)} should be less than {MaxValue}";
                failure = true;
            }

            if (validateAgainst.Counterparty.Value.Name.Length < MaxCounterpartyNameLength) {
                validateAgainst.Counterparty.Error = $"{nameof(validateAgainst.Counterparty)} name length should be" +
                                                     $" less than {MaxCounterpartyNameLength} characters";
                failure = true;
            }
        }

        return failure;
    }
}