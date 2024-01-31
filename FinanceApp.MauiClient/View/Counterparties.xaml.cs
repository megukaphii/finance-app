using FinanceApp.MauiClient.ViewModel;

namespace FinanceApp.MauiClient.View;

public partial class Counterparties : ContentPage
{
	public Counterparties(CounterpartiesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		viewModel.Title = "Counterparties";
	}

	protected override void OnAppearing()
	{
		//((CounterpartiesViewModel)BindingContext).GetCounterparties();
	}

	private void CounterpartyName_OnTextChanged(object? sender, TextChangedEventArgs textChangedEventArgs)
	{
		//((CounterpartiesViewModel)BindingContext).SearchCounterparties();
	}
}