using System.ComponentModel.DataAnnotations;
using FinanceApp.Data.Extensions;

namespace FinanceApp.Data.Models;

public class Account
{
    public static Account Empty { get; } = new() { Name = string.Empty, Description = string.Empty, Value = 0 };

    public long Id { get; init; }
    public List<Transaction> Transactions { get; init; } = new();
    [MinLength(2)]
    [MaxLength(64)]
    public required string Name { get; set; } = string.Empty;
    [MinLength(0)]
    [MaxLength(255)]
    public required string Description { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    public required decimal Value { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description.Truncate(64)}";
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