using FinanceApp.Data.Models;

namespace FinanceApp.Data.Interfaces;

public interface ISession
{
	Account Account { get; set; }
	bool IsAccountSet();
}