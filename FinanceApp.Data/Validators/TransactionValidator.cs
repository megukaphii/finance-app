using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Extensions;

namespace FinanceApp.Data.Validators;

public class TransactionValidator : IValidator
{
    private const double MinValue = double.MinValue;
    private const double MaxValue = double.MaxValue;
    private static readonly int MaxCounterpartyNameLength = Helpers.GetPropertyMaxLength((Counterparty c) => c.Name);

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