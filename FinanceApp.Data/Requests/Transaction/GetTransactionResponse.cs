using System.Text;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Transaction;

public class GetTransactionResponse : IResponse
{
	public required bool Success { get; init; }
	public required List<Models.Transaction> Transactions { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Transactions.Count}, [{nameof(Transactions)}: {string.Join(", ", Transactions)}]");
        return result.ToString();
    }
}