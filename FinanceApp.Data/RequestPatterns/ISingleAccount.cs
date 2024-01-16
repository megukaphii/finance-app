using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleAccount : IRequest
{
	public RequestField<string> Name { get; init; }
	public RequestField<string> Description { get; init; }
}