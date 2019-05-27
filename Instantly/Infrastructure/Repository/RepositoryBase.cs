using Instantly.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Instantly.Infrastructure.Repository
{
    public abstract class RepositoryBase<T, TDbContext> where T : class where TDbContext : DbContext
    {

        private readonly IDbSet<T> _dbset;
        private TDbContext _dataContext;

        protected RepositoryBase(IDatabaseFactory<TDbContext> databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = DataContext.Set<T>();
            // disable Code First default strategy
            Database.SetInitializer<TDbContext>(null);

            // TODO use environment variable in debug issues 
            // logging all sql queries
            //DataContext.Database.Log = s => _log.Debug(s);

        }

        protected IDatabaseFactory<TDbContext> DatabaseFactory { get; private set; }

        protected TDbContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.Get());

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void AddRange(IList<T> entities)
        {
            foreach (T entity in entities)
            {
                _dbset.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual void DeleteRange(IList<T> entities)
        {
            foreach (T entity in entities)
            {
                _dbset.Remove(entity);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = _dbset.Where(where);
            foreach (T obj in objects)
            {
                _dbset.Remove(obj);
            }
        }

        public virtual T GetById(decimal id)
        {
            return _dbset.Find(id);
        }
        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public virtual T GetById(string id)
        {
            return _dbset.Find(id);
        }

        public virtual IQueryable<T> GetAllAsQueryable()
        {
            return _dbset;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbset;
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where);
        }

        public virtual IQueryable<T> GetManyAsQueryable(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where);
        }

        /// <summary>
        ///     Return a paged list of entities
        /// </summary>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="page">Which page to retrieve</param>
        /// <param name="where">Where clause to apply</param>
        /// <param name="order">Order by to apply</param>
        /// <returns></returns>

        public virtual T Get(Expression<Func<T, bool>> where)
        {
            return _dbset.FirstOrDefault(where);
        }
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await _dbset.FirstOrDefaultAsync(where);
        }
        public virtual int SaveChanges()
        {
            return DataContext.SaveChanges();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }


        public virtual async Task<int> SaveChangesAsync()
        {
            return await DataContext.SaveChangesAsync();
        }
    }
}