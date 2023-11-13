using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface IAccountId : IRequest
{
    public new static Type Validator => typeof(AccountIdValidator);

    public RequestField<long> Id { get; }
}