using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Validators;

public class AccountValidator : IValidator
{
    // TODO - Are these cached (I don't think so)? Should they be cached (wouldn't hurt)?
    private static readonly int MinNameLength = Helpers.GetPropertyMinLength((Account a) => a.Name);
    private static readonly int MaxNameLength = Helpers.GetPropertyMaxLength((Account a) => a.Name);
    private static readonly int MinDescriptionLength = Helpers.GetPropertyMinLength((Account a) => a.Description);
    private static readonly int MaxDescriptionLength = Helpers.GetPropertyMaxLength((Account a) => a.Description);

    public Task<bool> ValidateAsync(IRequest request, FinanceAppContext db)
    {
        bool failure = false;
        if (request is ISingleAccount validateAgainst) {
            if (validateAgainst.Name.Value.Length < MinNameLength) {
                validateAgainst.Name.Error = $"{nameof(validateAgainst.Name)} should be more than {MinNameLength} characters";
                failure = true;
            } else if (validateAgainst.Name.Value.Length > MaxNameLength) {
                validateAgainst.Name.Error = $"{nameof(validateAgainst.Name)} should be less than {MaxNameLength} characters";
                failure = true;
            }

            if (validateAgainst.Description.Value.Length < MinDescriptionLength) {
                validateAgainst.Description.Error = $"{nameof(validateAgainst.Description)} should be more than {MinDescriptionLength} characters";
                failure = true;
            } else if (validateAgainst.Description.Value.Length > MaxDescriptionLength) {
                validateAgainst.Description.Error = $"{nameof(validateAgainst.Description)} should be less than {MaxDescriptionLength} characters";
                failure = true;
            }
        }

        return Task.FromResult(!failure);
    }
}