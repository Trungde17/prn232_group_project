using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate); 
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities);
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
        Task SaveChangesAsync();

    }
}
