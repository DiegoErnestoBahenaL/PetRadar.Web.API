using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class SystemConfigRepository : EntityRepository<SystemConfigEntity>, ISystemConfigRepository
    {
        public SystemConfigRepository(PetRadarDbContext db) : base(db, db.SystemConfigs) { }

        public Task<List<SystemConfigEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);
            return query.ToListAsync(token);
        }

        public Task<SystemConfigEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.SystemConfigs
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<SystemConfigEntity?> FindByKeyAsync(string key, CancellationToken token)
        {
            return _dbContext.SystemConfigs
                .SingleOrDefaultAsync(x => x.IsActive == true && x.Key == key, token);
        }
    }
}
