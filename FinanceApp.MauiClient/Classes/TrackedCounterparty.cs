using CommunityToolkit.Mvvm.ComponentModel;
using FinanceApp.Data.Models;

namespace FinanceApp.MauiClient.Classes;

public partial class TrackedCounterparty(bool isActive, Counterparty counterparty) : ObservableObject
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

	private Counterparty _original = new() { Id = counterparty.Id, Name = counterparty.Name };

	public void SaveChanges()
	{
		_original = new() { Id = Counterparty.Id, Name = Counterparty.Name };
	}

	public void UndoChanges()
	{
		CounterpartyName = _original.Name;
	}
}