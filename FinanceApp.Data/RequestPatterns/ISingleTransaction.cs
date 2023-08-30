using FinanceApp.Abstractions;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
    public static readonly Type? Validator = typeof(TransactionValidator);

    public int Value { get; }
    public string CounterpartyName { get; }
}