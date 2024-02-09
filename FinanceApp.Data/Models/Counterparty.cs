using System.ComponentModel.DataAnnotations;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Models;

public class Counterparty : IModel
{
	public static Counterparty Empty => new() { Id = -1, Name = "Select Counterparty..." };

	public long Id { get; init; }
	public List<Transaction> Transactions { get; init; } = new();
	public List<Subscription> Subscriptions { get; init; } = new();

	[MinLength(2)]
	[MaxLength(64)]
	public required string Name { get; set; }

	public override string ToString() => $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == typeof(Counterparty) && Equals((Counterparty)obj);
	}

	private bool Equals(Counterparty other) => (Id == other.Id || Id == 0 || other.Id == 0) && Name == other.Name;

	public override int GetHashCode() => HashCode.Combine(Id, Transactions);
}