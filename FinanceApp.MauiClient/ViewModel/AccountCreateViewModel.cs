using FinanceApp.MauiClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Requests.Account;
using FinanceApp.MauiClient.View;

namespace FinanceApp.MauiClient.ViewModel;

public partial class AccountCreateViewModel(ServerConnection serverConnection) : BaseViewModel(serverConnection)
{
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _nameError = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;
    [ObservableProperty]
    private string _descriptionError = string.Empty;

    [RelayCommand]
    private async Task CreateAccount()
    {
        try {
            IsBusy = true;
            ClearErrors();

            CreateAccount request = new()
            {
                Name = new()
                {
                    Value = Name
                },
                Description = new()
                {
                    Value = Description
                }
            };
            CreateAccountResponse response = await ServerConnection.SendMessageAsync<CreateAccount, CreateAccountResponse>(request);
            if (response.Success) {
                await Shell.Current.DisplayAlert("Created Transaction", $"Successfully created account {response}",
                    "OK");
            }
        } catch (ResponseException<CreateAccount> ex) {
            if (!string.IsNullOrEmpty(ex.Response.Name.Error)) {
                NameError = ex.Response.Name.Error;
            }

            if (!string.IsNullOrEmpty(ex.Response.Description.Error)) {
                NameError = ex.Response.Description.Error;
            }
        } catch (Exception ex) {
            await ServerConnection.DisconnectAsync();
            await Shell.Current.GoToAsync($"//{nameof(Login)}", true);
            await Shell.Current.DisplayAlert("Error", ex.Message + " | Inner exception: " + ex.InnerException?.Message,
                "OK");
        } finally {
            IsBusy = false;

            await Shell.Current.GoToAsync("..", true);
        }
    }

    public override void ClearErrors()
    {
        NameError = string.Empty;
        DescriptionError = string.Empty;
    }
}