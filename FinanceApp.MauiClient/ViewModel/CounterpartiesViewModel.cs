using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.MauiClient.Classes;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class CounterpartiesViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache)
{
	[ObservableProperty]
	private string _pageError = string.Empty;

	[ObservableProperty]
	private string _search = string.Empty;
	[ObservableProperty]
	private string _searchError = string.Empty;

	private TrackedCounterparty? _activeCounterparty;
	private TrackedCounterparty? ActiveCounterparty
	{
		get => _activeCounterparty;
		set
		{
			if (_activeCounterparty != null) _activeCounterparty.IsActive = false;
			_activeCounterparty = value;
		}
	}
	private List<Counterparty> Counterparties { get; } = [];
	public ObservableCollection<TrackedCounterparty> CounterpartiesSearch { get; set; } = [];

	[RelayCommand]
	private async Task SelectCounterparty(TrackedCounterparty selected)
	{
		ShellNavigationQueryParameters parameters = new() { { nameof(Counterparty), selected.Counterparty } };
		await Shell.Current.GoToAsync("..", true, parameters);
	}

	[RelayCommand]
	private void ActivateCounterparty(TrackedCounterparty selected)
	{
		ActiveCounterparty?.UndoChanges();
		ActiveCounterparty = selected;
		selected.IsActive = true;
	}

	[RelayCommand]
	public async Task GetCounterparties()
	{
		try {
			IsBusy = true;
			ClearErrors();

			GetCounterparties request = new()
			{
				Page = new()
				{
					Value = 0
				}
			};
			GetCounterpartiesResponse response =
				await ServerConnection.SendMessageAsync<GetCounterparties, GetCounterpartiesResponse>(request);

			Counterparties.Clear();
			foreach (Counterparty counterparty in response.Counterparties) Counterparties.Add(counterparty);
			SearchCounterparties();
		} catch (ResponseException<GetCounterparties> ex) {
			PageError = ex.Message;
		} catch (Exception ex) {
			await ServerConnection.DisconnectAsync();
			await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message,
				"OK");
		} finally {
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task UpdateCounterparty(TrackedCounterparty selected)
	{
		try {
			IsBusy = true;
			ClearErrors();

			UpdateCounterparty request = new()
			{
				Id = new() { Value = selected.Counterparty.Id },
				Name = new() { Value = selected.CounterpartyName }
			};
			UpdateCounterpartyResponse response =
				await ServerConnection.SendMessageAsync<UpdateCounterparty, UpdateCounterpartyResponse>(request);

			if (response.Success) {
				ActiveCounterparty?.SaveChanges();
				await Shell.Current.DisplayAlert("Updated", $"Successfully updated {selected.CounterpartyName}", "OK");
			}
		} catch (ResponseException<GetCounterparties> ex) {
			PageError = ex.Message;
		} catch (Exception ex) {
			await ServerConnection.DisconnectAsync();
			await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message,
				"OK");
		} finally {
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task DeleteCounterparty(TrackedCounterparty selected)
	{
		// TODO - Send delete counterparty request
		await Shell.Current.DisplayAlert("Delete", $"Deleting {selected.CounterpartyName}", "OK");
	}

	public void SearchCounterparties()
	{
		CounterpartiesSearch.Clear();
		ActiveCounterparty = null;
		IEnumerable<Counterparty> temp = Counterparties.Where(counterparty =>
			counterparty.Name.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
		);
		foreach (Counterparty counterparty in temp) CounterpartiesSearch.Add(new(false, counterparty));
	}

	public override void ClearErrors()
	{
		PageError = string.Empty;
		SearchError = string.Empty;
	}
}