using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;

namespace FinanceApp.Server.Interfaces;

public interface IUnitOfWork : IDisposable
{
	public IRepository<T> Repository<T>() where T : class, IModel;
	public void AttachAccount(Account account);
	public void SaveChanges();
}