using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IValidatorResolver
{
	IValidator<T> GetValidator<T>() where T : IRequest;
}