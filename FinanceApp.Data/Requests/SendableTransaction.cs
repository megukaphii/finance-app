namespace FinanceApp.Data.Requests;
public class SendableTransaction {
	public long ID { get; init; }
	public long Value { get; init; }
	public string Transactee { get; init; }

	public override string ToString()
	{
		return $"{nameof(ID)}: {ID}, {nameof(Value)}: {Value}, {nameof(Transactee)}: {Transactee}";
	}
}