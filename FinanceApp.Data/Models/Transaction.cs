namespace FinanceApp.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public Counterparty Counterparty { get; set; }
    public int Value { get; set; }
}