using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.Validators;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
    public static readonly Type? Validator = typeof(TransactionValidator);

    public int Value { get; }
    public Counterparty Counterparty { get; }
}