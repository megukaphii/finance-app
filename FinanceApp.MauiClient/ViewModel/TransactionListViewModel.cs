using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.MauiClient.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.Data.Exceptions;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class TransactionListViewModel(ServerConnection serverConnection) : BaseViewModel(serverConnection)
{
    [ObservableProperty]
    private string _pageError = string.Empty;

	public ObservableCollection<Transaction> Transactions { get; } = new();

    [RelayCommand]
	private async Task LoadTransactions()
	{
		if (IsBusy) return;

        try {
            IsBusy = true;
            GetPage request = new()
            {
                Page = new() { Value = 0 }
            };

            GetPageResponse response = await ServerConnection.SendMessageAsync<GetPage, GetPageResponse>(request);

            Transactions.Clear();
            foreach (Transaction transaction in response.Transactions) {
                // TODO - This fires off an event with each add, figure out how to add range instead (refer to MonkeyFinder James Montemagno tutorial)
                Transactions.Add(transaction);
            }
        } catch (ResponseException<GetPage> ex) {
            if (!string.IsNullOrEmpty(ex.Response.Page.Error)) {
                PageError = ex.Response.Page.Error;
            }
        } catch (Exception ex) {
            await ServerConnection.DisconnectAsync();
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
		} finally {
			IsBusy = false;
		}
	}
}