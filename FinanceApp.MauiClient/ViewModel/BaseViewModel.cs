using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public abstract partial class BaseViewModel(ServerConnection serverConnection, IMemoryCache cache) : ObservableObject
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	private bool _isBusy;
	[ObservableProperty]
	private string _title = string.Empty;

	public bool IsNotBusy => !IsBusy;

	protected readonly ServerConnection ServerConnection = serverConnection;
    protected readonly IMemoryCache Cache = cache;

    public abstract void ClearErrors();
}