using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiNativeClient.Services;

namespace FinanceApp.MauiNativeClient.ViewModel;

public partial class BaseViewModel(ServerConnection serverConnection) : ObservableObject
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotConnected))]
	private bool _isConnected = false;
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	private bool _isBusy;
	[ObservableProperty]
	private string _title;

	public bool IsNotBusy => !IsBusy;
	public bool IsNotConnected => !IsConnected;

	internal ServerConnection _serverConnection = serverConnection;
}