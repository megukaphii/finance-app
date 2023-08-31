using System.Text;
using FinanceApp.Data.Models;

namespace FinanceApp.Data.Requests;

public class ViewTransactionResponse
{
	public required bool Success { get; init; }
	public required List<Transaction> Transactions { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Transactions.Count}, {nameof(Transactions)}:");
        foreach (Transaction transaction in Transactions)
        {
	        result.Append($"\n{transaction}");
        }
        return result.ToString();
    }
}