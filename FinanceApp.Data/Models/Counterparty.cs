﻿using System.ComponentModel.DataAnnotations;

namespace FinanceApp.Data.Models;

public class Counterparty
{
    public long Id { get; init; }
    public List<Transaction> Transactions { get; init; } = new();
    [MinLength(2)]
    [MaxLength(64)]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(Counterparty)) return false;
        return Equals((Counterparty)obj);
    }

    private bool Equals(Counterparty other)
    {
        return (Id == other.Id || Id == 0 || other.Id == 0) && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}