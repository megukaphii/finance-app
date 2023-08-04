namespace FinanceApp.Data.Requests;

public class CreateTransactionResponse
{
    public required long Id { get; set; }
    public required bool Success { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Success)}: {Success}";
    }
}