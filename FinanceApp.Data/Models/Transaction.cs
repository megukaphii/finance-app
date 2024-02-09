using System.ComponentModel.DataAnnotations;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Models;

public class Transaction : IModel
{
	public long Id { get; init; }
	public Subscription? Subscription { get; init; }
	public required Account Account { get; init; }
	public required Counterparty Counterparty { get; init; }
	[DataType(DataType.Currency)]
	public required decimal Value { get; init; }
	public required DateTime Timestamp { get; init; }

	public override string ToString() =>
		$"{nameof(Id)}: {Id}, {nameof(Subscription)}: {Subscription}, [{nameof(Account)}: {Account}], [{nameof(Counterparty)}: {Counterparty}], {nameof(Value)}: {Value}, {nameof(Timestamp)}: {Timestamp}";

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((Transaction)obj);
	}

	private bool Equals(Transaction other) => (Id == other.Id || Id == 0 || other.Id == 0) &&
	                                          (Subscription == null || Subscription.Equals(other.Subscription)) &&
	                                          Account.Equals(other.Account) &&
	                                          Counterparty.Equals(other.Counterparty) && Value == other.Value;

	public override int GetHashCode() => HashCode.Combine(Id, Counterparty, Value);
}