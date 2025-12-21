using CC.Domain.Common;
using System.Linq.Expressions;

namespace CC.Domain.Repositories
{
    public interface IGenericRepository<T,TKey> where T : BaseEntity<TKey>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
            Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null, 
            string? includeProperties = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
