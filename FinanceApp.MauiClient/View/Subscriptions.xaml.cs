using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class Subscriptions : ContentPage
{
	public Subscriptions(SubscriptionsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Subscriptions";
	}

	protected override void OnAppearing()
	{
		((SubscriptionsViewModel)BindingContext).LoadSubscriptions();
	}
}