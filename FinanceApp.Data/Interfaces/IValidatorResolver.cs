namespace FinanceApp.Data.Interfaces;

public interface IValidatorResolver
{
	IValidator<T> GetValidator<T>() where T : IRequest;
}