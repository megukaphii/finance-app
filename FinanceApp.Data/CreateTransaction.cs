using FinanceApp.Abstractions;

namespace FinanceApp.Data;

public class CreateTransaction : IRequest {
    public required string Type { get; set; }
    public required int Value { get; set; }

	public static string GetFlag() {
		return "<CreateTransaction>";
	}

	public override string ToString() {
		return $"{nameof(Type)}: {Type}, {nameof(Value)}: {Value}";
	}
}