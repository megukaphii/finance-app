using System.Text;

namespace FinanceApp.Data.Requests;

public class ViewTransactionResponse
{
	public required bool Success { get; init; }
	public required List<SendableTransaction> Transactions { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"Transactions: {nameof(Success)}: {Success}, Count: {Transactions.Count}");
        foreach (SendableTransaction transaction in Transactions)
        {
	        result.Append($"\n{transaction}");
        }
        return result.ToString();
    }
}