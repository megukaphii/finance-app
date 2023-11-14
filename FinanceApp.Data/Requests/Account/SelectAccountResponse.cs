using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Account;

public class SelectAccountResponse : IResponse
{
    public required bool Success { get; init; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}";
    }
}