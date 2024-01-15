namespace FinanceApp.Data.Extensions;

public static class StringExtensions
{
	public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
	{
		if (maxLength < 0)
			throw new ArgumentOutOfRangeException(nameof(maxLength), $"{nameof(maxLength)} cannot be less than 0");
		return value?.Length > maxLength
			       ? value[..maxLength] + truncationSuffix
			       : value;
	}
}