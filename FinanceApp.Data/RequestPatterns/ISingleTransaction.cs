using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
    public new static Type? Validator { get; } = typeof(TransactionValidator);

    public RequestField<double> Value { get; }
    public RequestField<Counterparty> Counterparty { get; }
}