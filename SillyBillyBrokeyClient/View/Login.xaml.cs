using FinanceApp.MauiNativeClient.ViewModel;

namespace FinanceApp.MauiNativeClient.View;

public partial class Login : ContentPage
{
	public Login(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}