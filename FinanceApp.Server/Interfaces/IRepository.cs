using System.Linq.Expressions;
using FinanceApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace FinanceApp.Server.Interfaces;

public interface IRepository<T> where T : IModel
{
	IIncludableQueryable<T, object> Include(Expression<Func<T, object>> include);
	IIncludableQueryable<T, object> IncludeAll(params Expression<Func<T, object>>[] includes);
	IQueryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> expression);
	ValueTask<T?> FindAsync(long id);
	Task<T> FirstAsync(Expression<Func<T, bool>> predicate);
	Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
	Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
	IQueryable<T> Where(Expression<Func<T, bool>> predicate);
	Task<List<T>> AllAsync();
	Task AddAsync(T entity);
	void Delete(T entity);
}