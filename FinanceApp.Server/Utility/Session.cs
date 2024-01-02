using FinanceApp.Data.Models;

namespace FinanceApp.Server.Utility;

public class Session
{
	// TODO - Optional types?
	public Account Account { get; set; } = Account.Empty;

	public bool IsAccountSet() => !Account.Equals(Account.Empty);
}