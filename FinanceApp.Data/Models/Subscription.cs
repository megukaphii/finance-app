using System.ComponentModel.DataAnnotations;
using FinanceApp.Data.Enums;

namespace FinanceApp.Data.Models;

public class Subscription
{
	public long Id { get; init; }
	public List<Transaction> Transactions { get; init; } = new();

	[MinLength(2)]
	[MaxLength(64)]
	public required string Name { get; set; } = string.Empty;
	public int FrequencyCounter { get; init; }
	public Frequency FrequencyMeasure { get; init; }
	public DateTime StartDate { get; init; }
	public DateTime EndDate { get; init; }
	public required Account Account { get; init; }
	public required Counterparty Counterparty { get; init; }
	[DataType(DataType.Currency)]
	public required decimal Value { get; init; }

	public override string ToString() =>
		$"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(FrequencyCounter)}: {FrequencyCounter}, {nameof(FrequencyMeasure)}: {FrequencyMeasure}, {nameof(StartDate)}: {StartDate}, {nameof(EndDate)}: {EndDate}, [{nameof(Account)}: {Account}], [{nameof(Counterparty)}: {Counterparty}], {nameof(Value)}: {Value}";

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == typeof(Subscription) && Equals((Subscription)obj);
	}

	private bool Equals(Subscription other) => (Id == other.Id || Id == 0 || other.Id == 0) && Name == other.Name &&
	                                           FrequencyCounter == other.FrequencyCounter &&
	                                           FrequencyMeasure == other.FrequencyMeasure &&
	                                           StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate) &&
	                                           Account.Equals(other.Account) &&
	                                           Counterparty.Equals(other.Counterparty) && Value == other.Value;

	public override int GetHashCode()
	{
		HashCode hashCode = new();
		hashCode.Add(Id);
		hashCode.Add(Transactions);
		hashCode.Add(FrequencyCounter);
		hashCode.Add((int)FrequencyMeasure);
		hashCode.Add(StartDate);
		hashCode.Add(EndDate);
		hashCode.Add(Account);
		hashCode.Add(Counterparty);
		hashCode.Add(Value);
		return hashCode.ToHashCode();
	}
}