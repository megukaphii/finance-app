using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Models;
using FinanceApp.Data.Requests.Account;
using FinanceApp.MauiClient.Services;
using FinanceApp.MauiClient.View;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceApp.MauiClient.ViewModel;

public partial class AccountsViewModel(ServerConnection serverConnection, IMemoryCache cache)
	: BaseViewModel(serverConnection, cache)
{
	[ObservableProperty]
	private string _pageError = string.Empty;

	public ObservableCollection<Account> Accounts { get; } = [];

	public async Task LoadAccounts()
	{
		try {
			IsBusy = true;
			ClearErrors();

			GetAccounts request = new()
			{
				Page = new() { Value = 0 }
			};

			GetAccountsResponse response =
				await ServerConnection.SendMessageAsync<GetAccounts, GetAccountsResponse>(request);

			Accounts.Clear();
			foreach (Account account in response.Accounts) Accounts.Add(account);
		} catch (ResponseException<GetAccounts> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Page.Error)) PageError = ex.Response.Page.Error;
		} catch (Exception ex) {
			await ServerConnection.DisconnectAsync();
			await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
			await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
		} finally {
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task SelectAccount(Account account)
	{
		try {
			IsBusy = true;
			ClearErrors();

			SelectAccount request = new()
			{
				Id = new()
				{
					Value = account.Id
				}
			};
			SelectAccountResponse response =
				await ServerConnection.SendMessageAsync<SelectAccount, SelectAccountResponse>(request);
			if (response.Success)
				await Shell.Current.DisplayAlert("Created Transaction", "Successfully set account",
					"OK");
		} catch (ResponseException<SelectAccount> ex) {
			if (!string.IsNullOrEmpty(ex.Response.Id.Error)) PageError = ex.Response.Id.Error;
		} catch (Exception ex) {
			await ServerConnection.DisconnectAsync();
			await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
			await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message,
				"OK");
		} finally {
			IsBusy = false;
		}

		await Shell.Current.GoToAsync($"//{nameof(QuickAdd)}", true);
	}

	[RelayCommand]
	private Task CreateAccount() => Shell.Current.GoToAsync(nameof(AccountCreate), true);

	public override void ClearErrors()
	{
		PageError = string.Empty;
	}
}