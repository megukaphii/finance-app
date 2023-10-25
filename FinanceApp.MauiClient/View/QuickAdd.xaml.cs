using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class QuickAdd : ContentPage
{
	public QuickAdd(QuickAddViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Quick Add";
	}

    private void CounterpartyName_OnFocused(object? sender, FocusEventArgs e)
    {
        ((QuickAddViewModel)BindingContext).GetCounterparties();
    }
}