using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Transaction;

namespace FinanceApp.Data.Requests.Transaction;

public class CreateTransaction : ITransactionFields
{
	public static string Flag => "<CreateTransaction>";

	public required RequestField<decimal> Value { get; init; }
	public required RequestField<long> Counterparty { get; init; }
	public required RequestField<DateTime> Timestamp { get; init; }

	public override string ToString() => $"{Flag}[{nameof(Value)}: {Value}], [{nameof(Counterparty)}: {Counterparty}]";
}

public class CreateTransactionResponse : IResponse
{
	public required long Id { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
}