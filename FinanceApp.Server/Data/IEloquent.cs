namespace FinanceApp.Server.Data;

public interface IEloquent<T> {
	public T Save();

	public abstract static T? Find(int id);
}
