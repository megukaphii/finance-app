using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;

namespace FinanceApp.Data.Requests.Transaction;

public class CreateTransaction : ISingleTransaction
{
	public static string Flag => "<CreateTransaction>";

	public required RequestField<decimal> Value { get; init; }
	public required RequestField<Models.Counterparty> Counterparty { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
}

public class CreateTransactionResponse : IResponse
{
	public required long Id { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((CreateTransactionResponse)obj);
	}

	private bool Equals(CreateTransactionResponse other) =>
		(Id == other.Id || Id == 0 || other.Id == 0) && Success == other.Success;

	public override int GetHashCode() => HashCode.Combine(Success, Id);
}