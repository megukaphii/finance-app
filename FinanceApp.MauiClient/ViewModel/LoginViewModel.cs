using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class LoginViewModel(ServerConnection serverConnection) : BaseViewModel(serverConnection)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    private bool _isConnected;
	[ObservableProperty]
	private string _ipAddress = ServerConnection.DefaultAddress;

    public bool IsNotConnected => !IsConnected;

    [RelayCommand]
	private void SetLocalIp()
	{
		IpAddress = "127.0.0.1";
	}

	[RelayCommand]
	private void SetNasIp()
	{
		IpAddress = "macdonald9999.synology.me";
	}

	[RelayCommand]
	private async Task EstablishConnection()
	{
		try {
			IsBusy = true;
			if (await ServerConnection.EstablishConnection(IpAddress)) {
				IsConnected = ServerConnection.IsConnected;
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
			await ServerConnection.DisconnectAsync();
			IsConnected = ServerConnection.IsConnected;
			await Shell.Current.DisplayAlert("Disconnected", "Successfully disconnected from server!", "OK");
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}

    public void CheckConnection()
    {
        IsConnected = ServerConnection.IsConnected;
    }
}