using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class Transactions : ContentPage
{
	public Transactions(TransactionsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Transaction List";
	}

	protected override void OnAppearing()
	{
		((TransactionsViewModel)BindingContext).LoadTransactions();
	}
}