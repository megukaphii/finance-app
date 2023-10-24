namespace FinanceApp.Data.RequestPatterns;

public class RequestField<T>
{
    public required T Value { get; init; }
    public string Error { get; set; } = string.Empty;

    public override string ToString()
    {
        string errorStr = Error == string.Empty ? "" : $", {nameof(Error)}: {Error}";
        string valueStr = Value?.ToString() ?? string.Empty;
        if (!typeof(T).IsPrimitive) {
            valueStr = "[" + valueStr + "]";
        }
        return $"<{typeof(T).Name}>{nameof(Value)}: {valueStr}{errorStr}";
    }
}