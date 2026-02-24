using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IAdoptionAnimalRepository : IEntityRepository<AdoptionAnimalEntity>
    {
        Task<List<AdoptionAnimalEntity>> GetAllAsync(CancellationToken token);
        Task<List<AdoptionAnimalEntity>> GetAllByShelterIdAsync(long shelterId, CancellationToken token);
        Task<AdoptionAnimalEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
