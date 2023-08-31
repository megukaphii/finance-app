namespace FinanceApp.Data.Interfaces;

public interface IValidator
{
    public bool Validate(IRequest request);
}