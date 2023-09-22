using FinanceApp.MauiNativeClient.ViewModel;

namespace FinanceApp.MauiNativeClient.View;

public partial class QuickAdd : ContentPage
{
	public QuickAdd(QuickAddViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}