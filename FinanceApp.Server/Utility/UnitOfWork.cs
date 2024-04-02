using FinanceApp.Data.Interfaces;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Utility;

public class UnitOfWork : IUnitOfWork
{
	public UnitOfWork(DbContext context)
	{
		Context = context;
		Repositories = new();
	}

	private DbContext Context { get; }
	private Dictionary<string, dynamic> Repositories { get; }

	public IRepository<T> Repository<T>() where T : class, IModel
	{
		string type = typeof(T).Name;
		if (Repositories.TryGetValue(type, out dynamic? repository))
			return (IRepository<T>)repository;

		Type repositoryType = typeof(Repository<>);
		Repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), Context)
		                       ?? throw new InvalidOperationException());
		return Repositories[type];
	}

	public void SaveChanges()
	{
		Context.SaveChanges();
	}

	public void Dispose()
	{
		Context.Dispose();
		GC.SuppressFinalize(this);
	}
}