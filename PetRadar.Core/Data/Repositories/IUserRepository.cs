using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IUserRepository : IEntityRepository<UserEntity>
    {
        Task<List<UserEntity>> GetAllAsync(CancellationToken token);
        Task<UserEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<UserEntity?> FindByEmailAsync(string email, CancellationToken token);

    }
}
