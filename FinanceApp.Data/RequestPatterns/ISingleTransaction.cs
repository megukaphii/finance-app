using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
    public new static Type Validator => typeof(TransactionValidator);

    public RequestField<decimal> Value { get; init; }
    public RequestField<Counterparty> Counterparty { get; init; }
}