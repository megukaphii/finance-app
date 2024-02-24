using System.ComponentModel.DataAnnotations;
using FinanceApp.Data.Enums;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Models;

public class Subscription : IModel
{
	public long Id { get; init; }
	public List<Transaction> Transactions { get; init; } = new();

	public required Account Account { get; init; }
	public required Counterparty Counterparty { get; init; }
	[MinLength(2)]
	[MaxLength(64)]
	public required string Name { get; set; } = string.Empty;
	public required decimal Value { get; init; }
	[Range(1, 365)]
	public required int FrequencyCounter { get; init; }
	public required Frequency FrequencyMeasure { get; init; }
	public required DateTime StartDate { get; init; }
	public required DateTime EndDate { get; init; }

	[DataType(DataType.Currency)]
	public override string ToString() =>
		$"{nameof(Id)}: {Id}, [{nameof(Account)}: {Account}], [{nameof(Counterparty)}: {Counterparty}], {nameof(Name)}: {Name}, {nameof(Value)}: {Value}, {nameof(FrequencyCounter)}: {FrequencyCounter}, {nameof(FrequencyMeasure)}: {FrequencyMeasure}, {nameof(StartDate)}: {StartDate}, {nameof(EndDate)}: {EndDate}";

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