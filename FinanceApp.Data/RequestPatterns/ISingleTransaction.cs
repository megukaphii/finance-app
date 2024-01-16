using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;

namespace FinanceApp.Data.RequestPatterns;

public interface ISingleTransaction : IRequest
{
	public RequestField<decimal> Value { get; init; }
	public RequestField<Counterparty> Counterparty { get; init; }
}