using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class Accounts : ContentPage
{
    public Accounts(AccountsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.Title = "Accounts";
        ((AccountsViewModel)BindingContext).LoadAccounts();
    }
}