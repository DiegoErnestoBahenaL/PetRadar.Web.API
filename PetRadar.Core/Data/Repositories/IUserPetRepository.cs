using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IUserPetRepository : IEntityRepository<UserPetEntity>
    {
        Task<List<UserPetEntity>> GetAllAsync(CancellationToken token);
        Task<List<UserPetEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<UserPetEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
