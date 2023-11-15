using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class PageNumberValidator : IValidator
{
    private const long MinPage = 0;
    // This is just to stop it complaining about an unreachable switch case. Probably change it later.
    private const long MaxPage = long.MaxValue - 1;

    public bool Validate(IRequest request, FinanceAppContext db)
    {
        bool failure = false;
        if (request is IPageNumber validateAgainst) {
            switch (validateAgainst.Page.Value) {
                case < MinPage:
                    validateAgainst.Page.Error = $"{nameof(validateAgainst.Page)} should be greater than {MinPage}";
                    failure = true;
                    break;
                case > MaxPage:
                    validateAgainst.Page.Error = $"{nameof(validateAgainst.Page)} should be less than {MaxPage}";
                    failure = true;
                    break;
            }
        }

        return !failure;
    }
}