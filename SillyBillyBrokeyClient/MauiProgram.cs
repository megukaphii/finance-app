using FinanceApp.MauiNativeClient.Services;
using FinanceApp.MauiNativeClient.View;
using FinanceApp.MauiNativeClient.ViewModel;
using Microsoft.Extensions.Logging;

namespace FinanceApp.MauiNativeClient;
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

		builder.Services.AddSingleton<Login>();
		builder.Services.AddSingleton<QuickAdd>();

		return builder.Build();
	}
}
