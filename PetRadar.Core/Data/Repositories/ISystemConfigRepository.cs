using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface ISystemConfigRepository : IEntityRepository<SystemConfigEntity>
    {
        Task<List<SystemConfigEntity>> GetAllAsync(CancellationToken token);
        Task<SystemConfigEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<SystemConfigEntity?> FindByKeyAsync(string key, CancellationToken token);
    }
}
