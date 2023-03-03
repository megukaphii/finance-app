namespace FinanceApp.Abstractions;

public interface IParameter {
	T? GetValue<T>();
}