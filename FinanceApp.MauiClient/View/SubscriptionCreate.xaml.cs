using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class SubscriptionCreate : ContentPage
{
	public SubscriptionCreate(SubscriptionCreateViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Create Subscription";
	}
}