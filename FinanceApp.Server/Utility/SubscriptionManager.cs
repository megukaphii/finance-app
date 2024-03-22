using FinanceApp.Data.Enums;
using FinanceApp.Data.Models;
using FinanceApp.Server.Extensions;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Utility;

public class SubscriptionManager : ISubscriptionManager
{
	private readonly IUnitOfWork _unitOfWork;

	public SubscriptionManager(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

	public async Task ApplyDueSubscriptions()
	{
		List<Subscription> dueSubscriptions = await GetDueSubscriptions().ToListAsync();
		foreach (Transaction created in dueSubscriptions.Select(subscription => new Transaction
		         {
			         Subscription = subscription,
			         Account = subscription.Account,
			         Counterparty = subscription.Counterparty,
			         Value = subscription.Value,
			         Timestamp = DateTime.Today
		         })) {
			await _unitOfWork.AddTransaction(created);
			_unitOfWork.SaveChanges();
		}
	}

	public IQueryable<Subscription> GetActiveSubscriptions()
	{
		return _unitOfWork.Repository<Subscription>().Where(subscription =>
			subscription.StartDate <= DateTime.Today &&
			(subscription.EndDate >= DateTime.Today || subscription.EndDate.Equals(DateTime.UnixEpoch)));
	}

	private IQueryable<Subscription> GetDueSubscriptions()
	{
		DateTime currentDate = DateTime.Today;
		return GetActiveSubscriptions().Where(
			subscription => (
				                subscription.FrequencyMeasure == Frequency.Daily ? (currentDate - subscription.StartDate).TotalDays :
				                subscription.FrequencyMeasure == Frequency.Weekly ? (currentDate - subscription.StartDate).TotalDays / 7 :
				                subscription.FrequencyMeasure == Frequency.Monthly ? (currentDate.Year - subscription.StartDate.Year) * 12 +
				                                                                     (currentDate.Month - subscription.StartDate.Month) :
				                currentDate.Year - subscription.StartDate.Year)
			                % subscription.FrequencyCounter == 0);
	}
}