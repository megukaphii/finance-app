using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class Login : ContentPage
{
	public Login(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Login";
	}

	protected override void OnAppearing()
	{
		((LoginViewModel)BindingContext).CheckConnection();
	}
}