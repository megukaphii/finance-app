using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class QuickAddViewModel(ServerConnection serverConnection) : BaseViewModel(serverConnection)
{
	[ObservableProperty]
	private double _value;
    [ObservableProperty]
    private string _valueError = string.Empty;

	[ObservableProperty]
	private string _counterparty = string.Empty;
    [ObservableProperty]
    private string _counterpartyError = string.Empty;

    [RelayCommand]
	private async Task SendTransaction()
	{
        try {
            IsBusy = true;
            Create request = new()
            {
                Value = new()
                {
                    Value = Value
                },
                Counterparty = new()
                {
                    Value = new()
                    {
                        Name = Counterparty
                    }
                }
            };

            CreateResponse response = await ServerConnection.SendMessageAsync<Create, CreateResponse>(request);
            await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created transaction {response}",
                "OK");
        } catch (ResponseException<Create> ex) {
            if (!string.IsNullOrEmpty(ex.Response.Value.Error)) {
                ValueError = ex.Response.Value.Error;
            }

            if (!string.IsNullOrEmpty(ex.Response.Counterparty.Error)) {
                CounterpartyError = ex.Response.Counterparty.Error;
            }
        } catch (Exception ex) {
            await ServerConnection.DisconnectAsync();
            await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}
}