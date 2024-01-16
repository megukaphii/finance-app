namespace FinanceApp.Data.RequestPatterns;

public class RequestField<T>
{
	public required T Value { get; init; }
	public string Error { get; set; } = string.Empty;

	public override string ToString()
	{
		string valueStr = Value?.ToString() ?? string.Empty;
		if (!typeof(T).IsPrimitive) valueStr = "[" + valueStr + "]";
		string errorStr = Error == string.Empty ? "" : $", {nameof(Error)}: {Error}";

		return $"<{typeof(T).Name}>: {valueStr}{errorStr}";
	}
}