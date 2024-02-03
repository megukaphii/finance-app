﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class QuickAddViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache), IQueryAttributable
{
	[ObservableProperty]
	private string _pageError = string.Empty;

	[ObservableProperty]
	private decimal _value;
	[ObservableProperty]
	private string _valueError = string.Empty;

    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now.Date;
    [ObservableProperty]
    private string _timestampError = string.Empty;

    private Counterparty _counterparty = Counterparty.Empty;
    public Counterparty Counterparty
    {
	    get => _counterparty;
	    private set
	    {
		    _counterparty = value;
		    OnPropertyChanged();
	    }
    }
    [ObservableProperty]
    private string _counterpartyError = string.Empty;

    public void ApplyQueryAttributes(IDictionary<string, object?> query)
    {
	    query.TryGetValue(nameof(Data.Models.Counterparty), out object? temp);
	    if (temp != null) Counterparty = (Counterparty)temp;
    }

	[RelayCommand]
	private async Task SendTransaction()
	{
		if (Counterparty.Equals(Counterparty.Empty)) { CounterpartyError = "Please select a counterparty"; return; }

		try {
			IsBusy = true;
			ClearErrors();

			CreateTransaction request = new()
			{
				Value = new() { Value = Value },
				Counterparty = new() { Value = Counterparty.Id },
				Timestamp = new() { Value = Timestamp }
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
	private Task ViewCounterparties()
	{
		ClearErrors();
		return Shell.Current.GoToAsync(nameof(Counterparties), true);
	}

	public override void ClearErrors()
	{
		PageError = string.Empty;
		ValueError = string.Empty;
		TimestampError = string.Empty;
		CounterpartyError = string.Empty;
	}
}