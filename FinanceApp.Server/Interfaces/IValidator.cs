using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IValidator<in T> where T : IRequest
{
	public Task<bool> ValidateAsync(T request);
}