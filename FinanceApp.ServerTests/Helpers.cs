using FinanceApp.Data.Utility;

namespace FinanceApp.ServerTests;

public static class Helpers
{
	public static string RemoveFromEof(string input)
	{
		return input.Remove(input.LastIndexOf('>') + 1).Replace(Serialization.Eof, "");
	}
}