using FinanceApp.Data.Models;

namespace FinanceApp.Server.Interfaces;

public interface ISession
{
	Account Account { get; set; }
	bool IsAccountSet();
}