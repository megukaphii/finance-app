using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.Data.Models;

namespace FinanceApp.MauiClient.Classes;

public partial class ActiveCounterparty(bool isActive, Counterparty counterparty) : ObservableObject
{
	public Counterparty Counterparty { get; } = counterparty;

	public string CounterpartyName
	{
		get => Counterparty.Name;
		set
		{
			Counterparty.Name = value;
			OnPropertyChanged();
		}
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsInactive))]
	private bool _isActive = isActive;
	public bool IsInactive => !IsActive;
}