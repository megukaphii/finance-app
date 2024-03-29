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
		DateTime midnight = DateTime.Today.AddDays(1);
		TimeSpan timeUntilMidnight = midnight - now;
		_timer = new(RunSubscriptions, null, timeUntilMidnight, TimeSpan.FromDays(1));
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_timer.Change(Timeout.Infinite, 0);
		return Task.CompletedTask;
	}

	private async void RunSubscriptions(object? state)
	{
		try {
			using IServiceScope scope = _scopeFactory.CreateScope();
			ISubscriptionManager subscriptionManager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
			await subscriptionManager.ApplyDueSubscriptions();
		} catch (Exception ex) {
			Console.WriteLine(ex);
		}
	}
}