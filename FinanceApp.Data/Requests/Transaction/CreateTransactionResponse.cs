using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Transaction;

public class CreateTransactionResponse : IResponse
{
    public required bool Success { get; init; }
    public required long Id { get; init; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CreateTransactionResponse)obj);
    }

    private bool Equals(CreateTransactionResponse other)
    {
        return (Id == other.Id || Id == 0 || other.Id == 0) && Success == other.Success;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Success, Id);
    }
}