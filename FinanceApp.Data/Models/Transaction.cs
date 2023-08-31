namespace FinanceApp.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public required Counterparty Counterparty { get; set; }
    public required int Value { get; set; }

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
        return (Id == other.Id || Id == 0 || other.Id == 0) && Counterparty.Equals(other.Counterparty) && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Counterparty, Value);
    }
}