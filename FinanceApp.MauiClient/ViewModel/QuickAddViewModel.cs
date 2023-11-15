using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class QuickAddViewModel(ServerConnection serverConnection) : BaseViewModel(serverConnection)
{
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

    private List<Counterparty> Counterparties { get; } = new();
    public ObservableCollection<Counterparty> CounterpartiesSearch { get; set; } = new();

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
                }
            };
            CreateTransactionResponse transactionResponse = await ServerConnection.SendMessageAsync<CreateTransaction, CreateTransactionResponse>(request);
            await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created transaction {transactionResponse}",
                "OK");
        } catch (ResponseException<CreateTransaction> ex) {
            if (!string.IsNullOrEmpty(ex.Response.Value.Error)) {
                ValueError = ex.Response.Value.Error;
            }

            if (!string.IsNullOrEmpty(ex.Response.Counterparty.Error)) {
                CounterpartyError = ex.Response.Counterparty.Error;
            }
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
            // TODO - Should this mark as busy? It's kinda just a little helper that goes on in the background
            IsBusy = true;
            ClearErrors();

            GetCounterparties request = new()
            {
                Page = new()
                {
                    Value = 0
                }
            };
            GetCounterpartiesResponse response = await ServerConnection.SendMessageAsync<GetCounterparties, GetCounterpartiesResponse>(request);

            // TODO - Cache this for x amount of seconds/minutes/etc?
            Counterparties.Clear();
            foreach (Counterparty counterparty in response.Counterparties) {
                Counterparties.Add(counterparty);
            }
            SearchCounterparties();
        } catch (ResponseException<GetCounterparties>) {
            // TODO - Make error happen!
        } catch (Exception ex) {
            await ServerConnection.DisconnectAsync();
            await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
            await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message, "OK");
        } finally {
            IsBusy = false;
        }
    }

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
        foreach (Counterparty counterparty in temp) {
            CounterpartiesSearch.Add(counterparty);
        }
    }

    public override void ClearErrors()
    {
        ValueError = string.Empty;
        CounterpartyError = string.Empty;
    }
}