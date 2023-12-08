using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns;

public interface IAccountId : IRequest
{
	public RequestField<long> Id { get; init; }
}