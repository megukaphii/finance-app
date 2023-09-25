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
	private string _counterparty;

	public QuickAddViewModel(ServerConnection serverConnection) : base(serverConnection) { }

	[RelayCommand]
	private async Task SendTransaction()
	{
		try {
			IsBusy = true;
			Create transaction = new()
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

			CreateResponse test = await _serverConnection.SendButWithStrongCoupling(transaction);
			await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created transaction {test}", "OK");

			/*CreateResponse response = (CreateResponse) await _serverConnection.SendMessage(transaction, (message) => (IRequest) JsonConvert.DeserializeObject<CreateResponse>(message));
			await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created transaction {response}", "OK");*/
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}
}