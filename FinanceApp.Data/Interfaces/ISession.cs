namespace FinanceApp.Data.Interfaces;

public interface ISession
{
	long AccountId { get; set; }
	bool IsAccountSet();
}