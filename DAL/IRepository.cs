using DanfossTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DanfossTest.DAL
{
    public interface IRepository<T> : IDisposable
        where T : BaseModelMapping
    {
        IEnumerable<T> GetItems(params Expression<Func<T, object>>[] expressions);

        Task<IEnumerable<T>> GetItemsAsync(params Expression<Func<T, object>>[] expressions);

        T GetItem(Expression<Func<T, bool>> predicate);

        Task<T> GetMaxItemAsync();

        Task<T> GetMinItemAsync();

        void DeleteItem(int id);

        void SaveChange(T entity);

        void LoadRelatedEntiti(T entity);
    }
}
