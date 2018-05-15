using DanfossTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DanfossTest.DAL
{
    public abstract class Repository<T> : IRepository<T>
        where T : BaseModelMapping
    {
        protected DataBaseContext DataBaseContext { get; }

        public Repository(DbContextOptions<DataBaseContext> options)
        {
            DataBaseContext = new DataBaseContext(options);
        }

        public void DeleteItem(int id)
        {
            var entity = GetItem(s => s.Id == id);
            DataBaseContext.Set<T>().Remove(entity);
            DataBaseContext.SaveChanges();
        }

        public virtual T GetItem(Expression<Func<T, bool>> predicate) => DataBaseContext.Set<T>().FirstOrDefault(predicate);

        public abstract Task<T> GetMaxItemAsync();

        public abstract Task<T> GetMinItemAsync();

        public IEnumerable<T> GetItems(params Expression<Func<T, object>>[] expressions) =>
            GetIQueryableItems(expressions).ToList();

        public async Task<IEnumerable<T>> GetItemsAsync(params Expression<Func<T, object>>[] expressions) =>
            await GetIQueryableItems(expressions).ToListAsync();

        private IQueryable<T> GetIQueryableItems(params Expression<Func<T, object>>[] expressions)
        {
            IQueryable<T> dbSet = DataBaseContext.Set<T>();

            foreach (var param in expressions)
            {
                dbSet = dbSet.Include(param);
            }

            return dbSet;
        }

        public virtual void SaveChange(T entity)
        {
            DataBaseContext.Entry(entity).State = entity.Id == 0 ?
               EntityState.Added :
               EntityState.Modified;

            DataBaseContext.SaveChanges();
        }

        public abstract void LoadRelatedEntiti(T entity);

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DataBaseContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
