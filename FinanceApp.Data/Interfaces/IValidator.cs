namespace FinanceApp.Data.Interfaces;

public interface IValidator
{
	public Task<bool> ValidateAsync(IRequest request);
}