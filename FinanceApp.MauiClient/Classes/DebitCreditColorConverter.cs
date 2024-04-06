using System.Globalization;

namespace FinanceApp.MauiClient.Classes;

internal class DebitCreditColorConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null) {
			string s = (string)value;
			if (!string.IsNullOrEmpty(s)) return s.Contains('-') ? new Color(255, 0, 0) : new(255, 255, 255);
		}
		return new Color(255, 255, 255);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}