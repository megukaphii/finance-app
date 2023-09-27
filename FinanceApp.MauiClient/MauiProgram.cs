using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.ViewModel;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Logging;

namespace FinanceApp.MauiClient;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
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

		builder.Services.AddSingleton<ServerConnection>();

		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<QuickAddViewModel>();
		builder.Services.AddSingleton<TransactionListViewModel>();

		builder.Services.AddSingleton<Login>();
		builder.Services.AddSingleton<QuickAdd>();
		builder.Services.AddSingleton<TransactionList>();

		return builder.Build();
	}
}
