using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IUnitOfWork : IDisposable
{
	public IRepository<T> Repository<T>() where T : class, IModel;
	public void SaveChanges();
}