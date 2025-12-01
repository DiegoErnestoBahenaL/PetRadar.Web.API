using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public abstract class EntityRepository<T> : IEntityRepository<T> where T : class, IEntity
    {
        protected readonly PetRadarDbContext _dbContext;

        readonly DbSet<T> _collection;


        public EntityRepository(PetRadarDbContext context, DbSet<T> collection)
        {
            _dbContext = context;
            _collection = collection;
        }

        public virtual async Task AddAsync(T entity)
        {
            await _collection.AddAsync(entity, CancellationToken.None);
        }

        public virtual void Add(T entity)
        {
            _collection.Add(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is PostgresException pgEx)
                    if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)

                        throw new Exception("Cannot create duplicated data");

                throw;
            }

        }
        public void Remove(T entity)
        {
            _collection.Remove(entity);
        }

        public virtual void Update(T entity)
        {
            _collection.Update(entity);
        }

        IDbContextTransaction IEntityRepository<T>.BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        public virtual IQueryable<T> ConstructQuery()
        {
            var query = (_collection as IQueryable<T>);

            return query;
        }
    }
}
