namespace FinanceApp.Data.Extensions;

public static class StringExtensions
{
	public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…") =>
		value?.Length > maxLength
			? value[..maxLength] + truncationSuffix
			: value;
}