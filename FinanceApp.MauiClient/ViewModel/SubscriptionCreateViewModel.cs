using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Enums;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Account;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class SubscriptionCreateViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache), IQueryAttributable
{
	[ObservableProperty]
	private string _value = string.Empty;
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
	private async Task CreateAccount()
	{
		try {
			/*IsBusy = true;
			ClearErrors();

			CreateAccount request = new()
			{
				Name = new()
				{
					Value = Name
				},
				Description = new()
				{
					Value = EventLogTags.Description
				}
			};
			CreateAccountResponse response =
				await ServerConnection.SendMessageAsync<CreateAccount, CreateAccountResponse>(request);
			if (response.Success) {
				await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created account {response}",
					"OK");

				await Shell.Current.GoToAsync("..", true);
			}*/
		} catch (ResponseException<CreateAccount> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Name.Error)) NameError = ex.Response.Name.Error;

			if (!string.IsNullOrEmpty(ex.Response.Description.Error)) NameError = ex.Response.Description.Error;
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
		NameError = string.Empty;
		//DescriptionError = string.Empty;
	}
}