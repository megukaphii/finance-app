using System.Linq.Expressions;
using FinanceApp.Data.Interfaces;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FinanceApp.Server.Utility;

public class Repository<T> : IRepository<T> where T : class, IModel
{
	public Repository(DbContext context) => Context = context;

	private DbContext Context { get; }

	public IIncludableQueryable<T, object> Include(Expression<Func<T, object>> include) =>
		Context.Set<T>().Include(include);

	public IIncludableQueryable<T, object> IncludeAll(params Expression<Func<T, object>>[] includes)
	{
		IIncludableQueryable<T, object> result = Context.Set<T>().Include(includes.First());
		for (int i = 1; i < includes.Length; i++) result = result.Include(includes[i]);

		return result;
	}

	public IQueryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> expression) => Context.Set<T>().OrderBy(expression);

	public ValueTask<T?> FindAsync(long id) => Context.Set<T>().FindAsync(id);

	public Task<T> FirstAsync(Expression<Func<T, bool>> predicate) => Context.Set<T>().FirstAsync(predicate);

	public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
		Context.Set<T>().FirstOrDefaultAsync(predicate);

	public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => Context.Set<T>().AnyAsync(predicate);

	public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => Context.Set<T>().Where(predicate);

	public Task<List<T>> AllAsync() => Context.Set<T>().ToListAsync();

	public async Task AddAsync(T entity)
	{
		await Context.Set<T>().AddAsync(entity);
	}

	public void Delete(T entity)
	{
		Context.Set<T>().Remove(entity);
	}
}