using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.MauiClient.Services;

namespace FinanceApp.MauiClient.ViewModel;

public partial class TransactionListViewModel : BaseViewModel
{
	public TransactionListViewModel(ServerConnection serverConnection) : base(serverConnection) { }
}