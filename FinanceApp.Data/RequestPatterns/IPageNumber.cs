using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface IPageNumber : IRequest
{
    public static readonly Type? Validator = typeof(PageNumberValidator);

    public RequestField<long> Page { get; }
}