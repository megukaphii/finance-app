using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Data.Models;

public class Account
{
    public long Id { get; init; }
    public List<Transaction> Transactions { get; init; } = new();
    [MaxLength(64)]
    public required string Name { get; set; } = string.Empty;
    [MaxLength(255)]
    public required string Description { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    public required decimal Value { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description[..64] + (Description.Length > 64 ? "..." : "")}";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Account)obj);
    }

    private bool Equals(Account other)
    {
        return (Id == other.Id || Id == 0 || other.Id == 0) && Name.Equals(other.Name) && Description.Equals(other.Description);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}