using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Subscription;

public class GetSubscriptions : IPageNumber
{
	public static string Flag => "<GetSubscriptions>";

	public required RequestField<long> Page { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Page)}: {Page}]";
}

public class GetSubscriptionsResponse : IResponse
{
	public required List<Models.Subscription> Subscriptions { get; init; }
	public required bool Success { get; init; }

	public override string ToString() =>
		$"{nameof(Success)}: {Success}, Count: {Subscriptions.Count}, [{nameof(Subscriptions)}: {string.Join(", ", Subscriptions)}]";
}