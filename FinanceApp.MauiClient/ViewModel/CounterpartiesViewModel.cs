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

	private ActiveCounterparty? _activeCounterparty;
	private ActiveCounterparty? ActiveCounterparty
	{
		set
		{
			if (_activeCounterparty != null) _activeCounterparty.IsActive = false;
			_activeCounterparty = value;
		}
	}
	private List<Counterparty> Counterparties { get; } = [];
	public ObservableCollection<ActiveCounterparty> CounterpartiesSearch { get; set; } = [];

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
	private void SelectCounterparty(ActiveCounterparty? selected)
	{
		ActiveCounterparty = selected;
		if (selected != null) selected.IsActive = true;
	}

	[RelayCommand]
	private async Task UpdateCounterparty(ActiveCounterparty selected)
	{
		// TODO - Send update counterparty request
		await Shell.Current.DisplayAlert("Update", $"Updating {selected?.CounterpartyName}", "OK");
	}

	[RelayCommand]
	private async Task DeleteCounterparty(ActiveCounterparty selected)
	{
		// TODO - Send delete counterparty request
		await Shell.Current.DisplayAlert("Delete", $"Deleting {selected?.CounterpartyName}", "OK");
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