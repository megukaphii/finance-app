using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public class Session : ISession
{
	// TODO - Optional types?
	public long AccountId { get; set; } = 0;

	public bool IsAccountSet() => !AccountId.Equals(0);
}