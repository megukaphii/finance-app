namespace FinanceApp.Abstractions;

public interface IValidator
{
    public bool Validate(IRequest request);
}