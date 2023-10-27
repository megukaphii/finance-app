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

    private void CounterpartyName_OnTextChanged(object? sender, TextChangedEventArgs textChangedEventArgs)
    {
        ((QuickAddViewModel)BindingContext).SearchCounterparties();
    }

    private void CounterpartyName_OnFocused(object? sender, FocusEventArgs e)
    {
        ((QuickAddViewModel)BindingContext).CounterpartyFocused = true;
        ((QuickAddViewModel)BindingContext).GetCounterparties();
        //((QuickAddViewModel)BindingContext).GetCounterpartiesCommand.ExecuteAsync(null);
    }

    private void CounterpartyName_OnUnfocused(object? sender, FocusEventArgs e)
    {
        ((QuickAddViewModel)BindingContext).CounterpartyFocused = false;
    }
}