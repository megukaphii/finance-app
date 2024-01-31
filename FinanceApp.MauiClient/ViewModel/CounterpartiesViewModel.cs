using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Counterparty;
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

	private List<Counterparty> Counterparties { get; } = [];
	public ObservableCollection<Counterparty> CounterpartiesSearch { get; set; } = [];

	/*[RelayCommand]
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
	private void SelectCounterparty(Counterparty? selected)
	{
		//if (selected is not null) Counterparty = selected.Name;
	}

	public void SearchCounterparties()
	{
		CounterpartiesSearch.Clear();
		IEnumerable<Counterparty> temp = Counterparties.Where(counterparty =>
			counterparty.Name.Contains(Search, StringComparison.CurrentCultureIgnoreCase)
		);
		foreach (Counterparty counterparty in temp) CounterpartiesSearch.Add(counterparty);
	}*/

	public override void ClearErrors()
	{
		PageError = string.Empty;
		SearchError = string.Empty;
	}
}