using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;

namespace FinanceApp.MauiClient.ViewModel;

public abstract partial class BaseViewModel(ServerConnection serverConnection) : ObservableObject
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	private bool _isBusy;
	[ObservableProperty]
	private string _title = string.Empty;

	public bool IsNotBusy => !IsBusy;

	protected readonly ServerConnection ServerConnection = serverConnection;

    public abstract void ClearErrors();
}