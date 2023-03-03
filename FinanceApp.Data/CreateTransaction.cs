namespace FinanceApp.Data;

public class CreateTransaction {
    public required string Type { get; set; }
    public required int Value { get; set; }

	public override string ToString() {
		return $"{nameof(Type)}: {Type}, {nameof(Value)}: {Value}";
	}
}