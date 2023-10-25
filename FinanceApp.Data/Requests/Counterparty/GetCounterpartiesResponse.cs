using System.Text;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Counterparty;

public class GetCounterpartiesResponse : IResponse
{
	public required bool Success { get; init; }
	public required List<Models.Counterparty> Counterparties { get; init; }

    public override string ToString()
    {
        StringBuilder result = new($"{nameof(Success)}: {Success}, Count: {Counterparties.Count}, [{nameof(Counterparties)}: {string.Join(", ", Counterparties)}]");
        return result.ToString();
    }
}