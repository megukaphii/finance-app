using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class LoginViewModel : BaseViewModel
{
	[ObservableProperty]
	private string _ipAddress = ServerConnection.DEFAULT_ADDRESS;

	public LoginViewModel(ServerConnection serverConnection) : base(serverConnection) { }

	[RelayCommand]
	private async Task EstablishConnection()
	{
		try {
			IsBusy = true;
			if (await _serverConnection.EstablishConnection(IpAddress)) {
				IsConnected = _serverConnection.IsConnected;
				await Shell.Current.DisplayAlert("Connection Established", $"Successfully connected to {IpAddress}!", "OK");
				await Shell.Current.GoToAsync($"//{nameof(QuickAdd)}", true);
			}
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task Disconnect()
	{
		try {
			IsBusy = true;
			await _serverConnection.Disconnect();
			IsConnected = _serverConnection.IsConnected;
			await Shell.Current.DisplayAlert("Disconnected", "Successfully disconnected from server!", "OK");
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}
}