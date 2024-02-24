using FinanceApp.Data.Enums;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.RequestPatterns.Subscription;

public interface ISubscriptionFields : IRequest
{
	public RequestField<long> Counterparty { get; init; }
	public RequestField<string> Name { get; init; }
	public RequestField<decimal> Value { get; init; }
	public RequestField<int> FrequencyCounter { get; init; }
	public RequestField<Frequency> FrequencyMeasure { get; init; }
	public RequestField<DateTime> StartDate { get; init; }
	public RequestField<DateTime> EndDate { get; init; }
}