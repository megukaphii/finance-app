using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleCounterparty : IRequest
{
	public RequestField<long> Id { get; init; }
	public RequestField<string> Name { get; init; }
}