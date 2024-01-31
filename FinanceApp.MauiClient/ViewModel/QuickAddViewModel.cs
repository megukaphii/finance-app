using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class QuickAddViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache)
{
	[ObservableProperty]
	private string _pageError = string.Empty;

	[ObservableProperty]
	private decimal _value;
	[ObservableProperty]
	private string _valueError = string.Empty;

	[ObservableProperty]
	private string _counterparty = string.Empty;
	[ObservableProperty]
	private string _counterpartyError = string.Empty;
	[ObservableProperty]
	private bool _counterpartyFocused = true;

    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now.Date;
    [ObservableProperty]
    private string _timestampError = string.Empty;

    [ObservableProperty]
    private string _selectedCounterparty = "Select Counterparty...";

	private List<Counterparty> Counterparties { get; } = [];
	public ObservableCollection<Counterparty> CounterpartiesSearch { get; set; } = [];

	[RelayCommand]
	private async Task SendTransaction()
	{
		try {
			IsBusy = true;
			ClearErrors();

			CreateTransaction request = new()
			{
				Value = new()
				{
					Value = Value
				},
				Counterparty = new()
				{
					Value = new()
					{
						Name = Counterparty
					}
				},
				Timestamp = new()
				{
					Value = Timestamp
				}
			};
			CreateTransactionResponse transactionResponse =
				await ServerConnection.SendMessageAsync<CreateTransaction, CreateTransactionResponse>(request);
			await Shell.Current.DisplayAlert("Created Transaction",
				$"Successfully created transaction {transactionResponse}",
				"OK");
		} catch (ResponseException<CreateTransaction> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Value.Error)) ValueError = ex.Response.Value.Error;
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
	private Task ViewCounterparties() => Shell.Current.GoToAsync(nameof(View.Counterparties), true);

	[RelayCommand]
	private void SelectCounterparty(Counterparty? selected)
	{
		if (selected is not null) Counterparty = selected.Name;
	}

	public void SearchCounterparties()
	{
		CounterpartiesSearch.Clear();
		IEnumerable<Counterparty> temp = Counterparties.Where(counterparty =>
			counterparty.Name.Contains(Counterparty, StringComparison.CurrentCultureIgnoreCase)
		);
		foreach (Counterparty counterparty in temp) CounterpartiesSearch.Add(counterparty);
	}

	public override void ClearErrors()
	{
		PageError = string.Empty;
		ValueError = string.Empty;
		CounterpartyError = string.Empty;
	}
}