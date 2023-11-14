using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Account;

public class CreateAccountResponse : IResponse
{
    public required bool Success { get; init; }
    public required long Id { get; init; }

    public override string ToString()
    {
        return $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
    }
}