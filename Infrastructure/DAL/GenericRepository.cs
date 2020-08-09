using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        internal TtcDbContext context;
        internal DbSet<TEntity> dbSet;
        public GenericRepository (TtcDbContext context) {
            this.context = context;
            this.dbSet = context.Set<TEntity> ();
        }
        public async Task<TEntity> AddAsync (TEntity entity) {
            if (entity == null)
                throw new ArgumentNullException ("entity");

            await dbSet.AddAsync (entity);

            await context.SaveChangesAsync ();

            return entity;
        }

        public void Delete (TEntity entityToDelete) {
            if (context.Entry (entityToDelete).State == EntityState.Detached) {
                dbSet.Attach (entityToDelete);
            }

            dbSet.Remove (entityToDelete);

            context.SaveChanges ();
        }

        

        public IEnumerable<TEntity> GetAll () {
            IQueryable<TEntity> query = dbSet;

            return query.AsEnumerable ();
        }

        public TEntity GetById (int id) {
            return dbSet.Find (id);
        }

        public IQueryable<TEntity> GetWithInclude (Expression<Func<TEntity, bool>> filter, string includeProperties) {
            IQueryable<TEntity> query = dbSet;
            if (filter != null) {
                query = query.Where (filter);
            }
            if (!string.IsNullOrEmpty (includeProperties)) {
                foreach (var property in includeProperties.Split (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include (property);
                }
            }

            return query;
        }

       

        public TEntity UpdateAsync (TEntity entityToUpdate) {
            if (entityToUpdate == null)
                throw new ArgumentNullException ("entity");

            dbSet.Attach (entityToUpdate);

            context.Entry (entityToUpdate).State = EntityState.Modified;

            return entityToUpdate;
        }
    }
}