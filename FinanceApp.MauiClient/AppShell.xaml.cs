using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Login), typeof(Login));
		Routing.RegisterRoute(nameof(QuickAdd), typeof(QuickAdd));
		Routing.RegisterRoute(nameof(TransactionList), typeof(TransactionList));
	}
}