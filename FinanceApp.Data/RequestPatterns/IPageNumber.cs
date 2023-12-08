using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns;

public interface IPageNumber : IRequest
{
	public RequestField<long> Page { get; init; }
}