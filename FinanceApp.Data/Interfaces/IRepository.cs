using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace FinanceApp.Data.Interfaces;

public interface IRepository<T> where T : IModel
{
	ValueTask<T?> FindAsync(long id);
	Task<T> FirstAsync(Expression<Func<T, bool>> predicate);
	Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
	Task<List<T>> AllAsync();
	Task AddAsync(T entity);
	IIncludableQueryable<T, object> Include(Expression<Func<T, object>> include);
	IIncludableQueryable<T, object> IncludeAll(params Expression<Func<T, object>>[] includes);
	void Update(T entity);
	void Delete(T entity);
	void Attach(T entity);
}