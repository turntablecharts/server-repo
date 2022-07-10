using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces {
    public interface IGenericRepository<TEntity> where TEntity : class {
        Task<TEntity> AddAsync (TEntity entity);

        Task<List<TEntity>> AddRange(List<TEntity> entity);
        TEntity GetById (int id);
        void Delete (TEntity entity);

        IQueryable<TEntity> GetAll ();
        TEntity UpdateAsync (TEntity entity);

        IQueryable<TEntity> GetWithInclude (Expression<Func<TEntity, bool>> filter, string includeProperties);

      
    }
}