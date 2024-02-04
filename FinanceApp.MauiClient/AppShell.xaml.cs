using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Login), typeof(Login));
		Routing.RegisterRoute(nameof(Accounts), typeof(Accounts));
		Routing.RegisterRoute(nameof(AccountCreate), typeof(AccountCreate));
		Routing.RegisterRoute(nameof(QuickAdd), typeof(QuickAdd));
		Routing.RegisterRoute(nameof(Counterparties), typeof(Counterparties));
		Routing.RegisterRoute(nameof(Transactions), typeof(Transactions));
	}
}