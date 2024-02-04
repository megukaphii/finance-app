using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns.Account;

public interface IAccountFields : IRequest
{
	public RequestField<string> Name { get; init; }
	public RequestField<string> Description { get; init; }
}