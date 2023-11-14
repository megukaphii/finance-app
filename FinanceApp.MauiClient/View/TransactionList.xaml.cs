using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class TransactionList : ContentPage
{
	public TransactionList(TransactionListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Transaction List";
	}

    protected override void OnAppearing()
    {
        ((TransactionListViewModel)BindingContext).LoadTransactions();
    }
}