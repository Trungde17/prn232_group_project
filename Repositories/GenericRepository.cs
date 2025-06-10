using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Homestays;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HomestayDbContext context;

        public GenericRepository(HomestayDbContext context)
        {
            this.context = context;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            return await context.SaveChangesAsync() > 0 ? entity : null;
        }

        public virtual async Task<IEnumerable<T>> AllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            context.Remove(entity);
            return await context.SaveChangesAsync() > 0 ? entity : null;
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().Where(predicate).ToListAsync();
        }
        public virtual async Task<T> GetWithConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public virtual async Task<T> GetAsync(dynamic id)
        {
            return await context.FindAsync<T>(id);
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            await context.Set<T>().AddRangeAsync(entities);
            return await context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
            return await context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
            return await context.SaveChangesAsync() > 0;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            context.Update(entity);
            return await context.SaveChangesAsync() > 0 ? entity : null;
        }
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }
    }
}
