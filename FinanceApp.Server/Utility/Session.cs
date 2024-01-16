using FinanceApp.Data.Models;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Utility;

public class Session : ISession
{
	// TODO - Optional types?
	public Account Account { get; set; } = Account.Empty;

	public bool IsAccountSet() => !Account.Equals(Account.Empty);
}