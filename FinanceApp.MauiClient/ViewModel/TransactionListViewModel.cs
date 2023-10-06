using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.MauiClient.Services;
using System.Collections.ObjectModel;

namespace FinanceApp.MauiClient.ViewModel;

public partial class TransactionListViewModel : BaseViewModel
{
	public ObservableCollection<Transaction> Transactions { get; } = new();

	public TransactionListViewModel(ServerConnection serverConnection) : base(serverConnection) { }

	[RelayCommand]
	private async Task LoadTransactions()
	{
		if (IsBusy) return;

		try {
			IsBusy = true;
			GetPage request = new()
			{
				Page = new RequestField<long> { Value = 0 }
			};

			GetPageResponse response = await _serverConnection.SendMessageAsync<GetPage, GetPageResponse>(request);

			Transactions.Clear();
			foreach (Transaction transaction in response.Transactions) {
				// TODO - this fires off an event with each add, figure out how to add range instead (refer to MonkeyFinder James Montemagno tutorial)
				Transactions.Add(transaction);
			}
		} catch (Exception ex) {
			await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
		} finally {
			IsBusy = false;
		}
	}
}