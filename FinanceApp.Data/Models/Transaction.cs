using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Models;

public class Transaction
{
    public long Id { get; init; }
    public required Counterparty Counterparty { get; init; }
    public required double Value { get; init; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, [{nameof(Counterparty)}: {Counterparty}], {nameof(Value)}: {Value}";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Transaction)obj);
    }

    private bool Equals(Transaction other)
    {
        return (Id == other.Id || Id == 0 || other.Id == 0) && Counterparty.Equals(other.Counterparty) && Math.Abs(Value - other.Value) < FloatCompare.Tolerance;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Counterparty, Value);
    }
}