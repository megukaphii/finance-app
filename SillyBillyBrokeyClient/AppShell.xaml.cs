using FinanceApp.MauiNativeClient.View;

namespace FinanceApp.MauiNativeClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(QuickAdd), typeof(QuickAdd));
	}
}
