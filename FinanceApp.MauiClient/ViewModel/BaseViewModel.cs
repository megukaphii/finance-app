using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public abstract partial class BaseViewModel(ServerConnection serverConnection, IMemoryCache cache) : ObservableObject
{
	protected readonly IMemoryCache Cache = cache;
	protected readonly ServerConnection ServerConnection = serverConnection;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	private bool _isBusy;

	[ObservableProperty]
	private string _title = string.Empty;

	public bool IsNotBusy => !IsBusy;

	public abstract void ClearErrors();
}