using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Extensions;

namespace FinanceApp.Data.Validators;

public class TransactionValidator : IValidator
{
    private const double MinValue = double.MinValue;
    private const double MaxValue = double.MaxValue;
    private static readonly int MinCounterpartyNameLength = Helpers.GetPropertyMinLength((Counterparty c) => c.Name);
    private static readonly int MaxCounterpartyNameLength = Helpers.GetPropertyMaxLength((Counterparty c) => c.Name);

    public bool Validate(IRequest request)
    {
        bool failure = false;
        if (request is ISingleTransaction validateAgainst) {
            switch (validateAgainst.Value.Value) {
                case < MinValue:
                    validateAgainst.Value.Error = $"{nameof(validateAgainst.Value)} should be greater than {MinValue}";
                    failure = true;
                    break;
                case > MaxValue:
                    validateAgainst.Value.Error = $"{nameof(validateAgainst.Value)} should be less than {MaxValue}";
                    failure = true;
                    break;
            }

            if (validateAgainst.Counterparty.Value.Name.Length < MinCounterpartyNameLength) {
                validateAgainst.Counterparty.Error = $"{nameof(validateAgainst.Counterparty)} name length should be" +
                                                     $" more than {MinCounterpartyNameLength} characters";
                failure = true;
            } else if (validateAgainst.Counterparty.Value.Name.Length > MaxCounterpartyNameLength) {
                validateAgainst.Counterparty.Error = $"{nameof(validateAgainst.Counterparty)} name length should be" +
                                                     $" less than {MaxCounterpartyNameLength} characters";
                failure = true;
            }
        }

        return !failure;
    }
}