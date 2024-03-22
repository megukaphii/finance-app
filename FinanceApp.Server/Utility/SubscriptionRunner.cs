using FinanceApp.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinanceApp.Server.Utility;

public class SubscriptionRunner : IHostedService, IDisposable
{
	private readonly IServiceScopeFactory _scopeFactory;
	private Timer _timer = null!;

	public SubscriptionRunner(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

	public void Dispose()
	{
		_timer.Dispose();
		GC.SuppressFinalize(this);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		DateTime now = DateTime.Now;
		DateTime midnight = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
		TimeSpan timeUntilMidnight = midnight - now;
		_timer = new(RunSubscriptions, null, timeUntilMidnight, TimeSpan.FromDays(1));
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_timer.Change(Timeout.Infinite, 0);
		return Task.CompletedTask;
	}

	private void RunSubscriptions(object? state)
	{
		using IServiceScope scope = _scopeFactory.CreateScope();
		ISubscriptionManager subscriptionManager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
		subscriptionManager.ApplyDueSubscriptions();
	}
}