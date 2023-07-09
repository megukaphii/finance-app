namespace FinanceApp.Server.Data;

internal interface IEloquentRepository<T> where T : Eloquent<T>, new()
{
    public T Find(int id);
    public List<T> All();
}