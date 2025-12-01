using Microsoft.EntityFrameworkCore.Storage;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IEntityRepository<T> where T : class, IEntity
    {
        Task<int> SaveChangesAsync();
        Task AddAsync(T entity);
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
        IQueryable<T> ConstructQuery();
        IDbContextTransaction BeginTransaction();
    }
}
