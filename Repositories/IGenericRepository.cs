using System.Linq.Expressions;
using BusinessObjects.Homestays;

namespace Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
        Task<T> GetAsync(dynamic id);
        Task<T> GetWithConditionAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> AllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        IQueryable<Homestay> Find(Expression<Func<Homestay, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        Task<int> AddRangesAsync(IEnumerable<T> entities);

        Task<bool> UpdateRangeAsync(IEnumerable<T> entities);
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
        Task SaveChangesAsync();

    }
}
