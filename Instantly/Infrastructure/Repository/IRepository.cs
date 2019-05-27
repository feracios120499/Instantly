using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Infrastructure.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void AddRange(IList<T> entities);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        void DeleteRange(IList<T> entities);
        T GetById(decimal id);
        T GetById(long id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        Task<T> GetAsync(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IQueryable<T> GetAllAsQueryable();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IQueryable<T> GetManyAsQueryable(Expression<Func<T, bool>> where);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<IEnumerable<T>> GetAllAsync();
    }
}