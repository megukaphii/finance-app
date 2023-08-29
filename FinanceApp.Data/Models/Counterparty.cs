namespace FinanceApp.Data.Models;

public class Counterparty
{
    public int Id { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
    public string Name { get; set; }
}