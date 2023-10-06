namespace FinanceApp.ServerTests;

public class Helpers
{
	public static string RemoveFromEof(string input)
	{
		return input.Remove(input.LastIndexOf('>') + 1).Replace("<EOF>", "");
	}
}