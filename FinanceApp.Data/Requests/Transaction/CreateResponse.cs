namespace FinanceApp.Data.Requests.Transaction;

public class CreateResponse
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
        return Equals((CreateResponse)obj);
    }

    private bool Equals(CreateResponse other)
    {
        return (Id == other.Id || Id == 0 || other.Id == 0) && Success == other.Success;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Success, Id);
    }
}