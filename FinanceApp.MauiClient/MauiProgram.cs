using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using FinanceApp.MauiClient.ViewModel;
using Microsoft.Extensions.Logging;

namespace FinanceApp.MauiClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddMemoryCache();
		builder.Services.AddSingleton<ServerConnection>();

		builder.Services.AddSingleton<Accounts>();
		builder.Services.AddSingleton<Login>();
		builder.Services.AddSingleton<Transactions>();

		builder.Services.AddSingleton<AccountsViewModel>();
		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<TransactionsViewModel>();

		builder.Services.AddTransient<AccountCreate>();
		builder.Services.AddTransient<Counterparties>();
		builder.Services.AddTransient<QuickAdd>();

		builder.Services.AddTransient<AccountCreateViewModel>();
		builder.Services.AddTransient<CounterpartiesViewModel>();
		builder.Services.AddTransient<QuickAddViewModel>();

		return builder.Build();
	}
}