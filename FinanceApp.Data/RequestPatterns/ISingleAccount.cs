using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleAccount : IRequest
{
    public new static Type Validator => typeof(AccountValidator);

    public RequestField<string> Name { get; init; }
    public RequestField<string> Description { get; init; }
}