namespace FinanceApp.DatabaseInterfaces;

public interface IParameter {
	T? GetValue<T>();
}