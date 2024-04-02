using FinanceApp.Data.Models;

namespace FinanceApp.Server.Interfaces;

public interface ISubscriptionManager
{
	public Task ApplyDueSubscriptions();
	public IQueryable<Subscription> GetActiveSubscriptions();
}