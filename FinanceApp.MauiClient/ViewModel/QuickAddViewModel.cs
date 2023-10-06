using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;

namespace FinanceApp.MauiClient.ViewModel;

public partial class QuickAddViewModel : BaseViewModel
{
	[ObservableProperty]
	private double _value;

	[ObservableProperty]
	private string _counterparty = string.Empty;

	public QuickAddViewModel(ServerConnection serverConnection) : base(serverConnection) { }

	[RelayCommand]
	private async Task SendTransaction()
	{
		try {
			IsBusy = true;
			Create request = new()
			{
				Value = new RequestField<double>
				{
					Value = Value
				},
				Counterparty = new RequestField<Counterparty>
				{
					Value = new Counterparty
					{
						Name = Counterparty
					}
				}
			};

			CreateResponse response = await _serverConnection.SendMessageAsync<Create, CreateResponse>(request);
			await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created transaction {response}", "OK");
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}
}