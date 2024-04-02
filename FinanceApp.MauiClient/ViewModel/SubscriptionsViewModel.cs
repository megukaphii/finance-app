using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class SubscriptionsViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache)
{
	[ObservableProperty]
	private string _pageError = string.Empty;

	public ObservableCollection<Subscription> Subscriptions { get; } = [];

	[RelayCommand]
	public async Task LoadSubscriptions()
	{
		if (IsBusy) return;

		try {
			IsBusy = true;
			ClearErrors();

			GetSubscriptions request = new()
			{
				Page = new() { Value = 0 }
			};

			GetSubscriptionsResponse response =
				await ServerConnection.SendMessageAsync<GetSubscriptions, GetSubscriptionsResponse>(request);

			Subscriptions.Clear();
			foreach (Subscription subscription in response.Subscriptions)
				// TODO - This fires off an event with each add, figure out how to add range instead (refer to MonkeyFinder James Montemagno tutorial)
				Subscriptions.Add(subscription);
		} catch (ResponseException<GetTransactions> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Page.Error)) PageError = ex.Response.Page.Error;
		} catch (Exception ex) {
			await ServerConnection.DisconnectAsync();
			await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
			await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
		} finally {
			IsBusy = false;
		}
	}

	[RelayCommand]
	private Task CreateSubscription() => Shell.Current.GoToAsync(nameof(SubscriptionCreate), true);

	public override void ClearErrors()
	{
		PageError = string.Empty;
	}
}