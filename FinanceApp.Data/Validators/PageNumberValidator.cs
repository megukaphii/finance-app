using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Validators;

public class PageNumberValidator
{
    private const long MinPage = 0;
    private const long MaxPage = long.MinValue;

    public bool Validate(IRequest request)
    {
        bool failure = false;
        if (request is IPageNumber validateAgainst) {
            if (validateAgainst.Page.Value < MinPage) {
                validateAgainst.Page.Error = $"{nameof(validateAgainst.Page)} should be greater than {MinPage}";
                failure = true;
            } else if (validateAgainst.Page.Value > MaxPage) {
                validateAgainst.Page.Error = $"{nameof(validateAgainst.Page)} should be less than {MaxPage}";
                failure = true;
            }
        }

        return !failure;
    }
}