using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Enums;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Subscription;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class SubscriptionCreateViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache), IQueryAttributable
{
	[ObservableProperty]
	private bool _isDebit = true;

	[ObservableProperty]
	private decimal _value;
	[ObservableProperty]
	private string _valueError = string.Empty;

	private Counterparty _counterparty = Counterparty.Empty;
	[ObservableProperty]
	private string _counterpartyError = string.Empty;

	[ObservableProperty]
	private string _name = string.Empty;
	[ObservableProperty]
	private string _nameError = string.Empty;

	[ObservableProperty]
	private int _frequencyCounter;
	[ObservableProperty]
	private string _frequencyCounterError = string.Empty;

	[ObservableProperty]
	private string _frequencyMeasure = Frequency.Daily.ToString();
	[ObservableProperty]
	private string _frequencyMeasureError = string.Empty;

	[ObservableProperty]
	private DateTime _startDate = DateTime.Today;
	[ObservableProperty]
	private string _startDateError = string.Empty;

	[ObservableProperty]
	private DateTime _endDate = DateTime.UnixEpoch;
	[ObservableProperty]
	private string _endDateError = string.Empty;
	public Counterparty Counterparty
	{
		get => _counterparty;
		private set
		{
			_counterparty = value;
			OnPropertyChanged();
		}
	}

	public List<string> Frequencies { get; } =
		Enum.GetValues(typeof(Frequency)).Cast<Frequency>().Select(e => e.ToString()).ToList();

	public void ApplyQueryAttributes(IDictionary<string, object?> query)
	{
		query.TryGetValue(nameof(Data.Models.Counterparty), out object? temp);
		if (temp != null) Counterparty = (Counterparty)temp;
	}

	[RelayCommand]
	private async Task CreateSubscription()
	{
		try {
			IsBusy = true;
			ClearErrors();

			if (!Enum.TryParse(FrequencyMeasure, out Frequency frequency)) {
				FrequencyMeasureError = "Invalid time frame";
			} else {
				CreateSubscription request = new()
				{
					Counterparty = new() { Value = Counterparty.Id },
					Name = new() { Value = Name },
					Value = new() { Value = Value * (IsDebit ? -1 : 1) },
					FrequencyCounter = new() { Value = FrequencyCounter },
					FrequencyMeasure = new() { Value = frequency },
					StartDate = new() { Value = StartDate },
					EndDate = new() { Value = EndDate }
				};
				CreateSubscriptionResponse response =
					await ServerConnection.SendMessageAsync<CreateSubscription, CreateSubscriptionResponse>(request);

				if (response.Success) {
					await Shell.Current.DisplayAlert("Created Subscription",
						$"Successfully created subscription {response}", "OK");
					await Shell.Current.GoToAsync("..", true);
				}
			}
		} catch (ResponseException<CreateSubscription> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Value.Error)) ValueError = ex.Response.Value.Error;
			if (!string.IsNullOrEmpty(ex.Response.Counterparty.Error))
				CounterpartyError = ex.Response.Counterparty.Error;
			if (!string.IsNullOrEmpty(ex.Response.Name.Error)) NameError = ex.Response.Name.Error;
			if (!string.IsNullOrEmpty(ex.Response.FrequencyCounter.Error))
				FrequencyCounterError = ex.Response.FrequencyCounter.Error;
			if (!string.IsNullOrEmpty(ex.Response.FrequencyMeasure.Error))
				FrequencyMeasureError = ex.Response.FrequencyMeasure.Error;
			if (!string.IsNullOrEmpty(ex.Response.StartDate.Error)) StartDateError = ex.Response.StartDate.Error;
			if (!string.IsNullOrEmpty(ex.Response.EndDate.Error)) EndDateError = ex.Response.EndDate.Error;
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
		ShellNavigationQueryParameters parameters = new() { { nameof(CounterpartiesViewModel.AllowSelect), true } };
		return Shell.Current.GoToAsync(nameof(Counterparties), true, parameters);
	}

	public override void ClearErrors()
	{
		ValueError = string.Empty;
		CounterpartyError = string.Empty;
		NameError = string.Empty;
		FrequencyCounterError = string.Empty;
		FrequencyMeasureError = string.Empty;
		StartDateError = string.Empty;
		EndDateError = string.Empty;
	}
}