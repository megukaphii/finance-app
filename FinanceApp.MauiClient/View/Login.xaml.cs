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
        // TODO - This seems weird? But apparently lifecycle events shouldn't go on the view model, so?
        ((LoginViewModel)BindingContext).CheckConnection();
    }
}