using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface IPageNumber : IRequest
{
    public new static Type Validator => typeof(PageNumberValidator);

    public RequestField<long> Page { get; init; }
}