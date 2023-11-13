using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class AccountCreate : ContentPage
{
    public AccountCreate(AccountCreateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.Title = "Create Account";
    }
}