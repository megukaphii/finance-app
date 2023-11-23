using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.ViewModel;
using FinanceApp.MauiClient.View;
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

		builder.Services.AddSingleton<Login>();
        builder.Services.AddSingleton<Accounts>();
		builder.Services.AddSingleton<QuickAdd>();
		builder.Services.AddSingleton<TransactionList>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<AccountsViewModel>();
        builder.Services.AddSingleton<QuickAddViewModel>();
        builder.Services.AddSingleton<TransactionListViewModel>();

        builder.Services.AddTransient<AccountCreate>();

        builder.Services.AddTransient<AccountCreateViewModel>();

		return builder.Build();
	}
}
