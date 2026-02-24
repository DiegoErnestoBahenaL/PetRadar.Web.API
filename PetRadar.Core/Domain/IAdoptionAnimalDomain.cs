using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IAdoptionAnimalDomain
    {
        Task<List<AdoptionAnimalEntity>> GetAllAsync(CancellationToken token);
        Task<List<AdoptionAnimalEntity>> GetAllByShelterIdAsync(long shelterId, CancellationToken token);
        Task<AdoptionAnimalEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<AdoptionAnimalEntity> CreateAsync(AdoptionAnimalCreateModel animal, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(AdoptionAnimalEntity animalDb, AdoptionAnimalUpdateModel animal, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(AdoptionAnimalEntity animal, long modifiedByUserId, CancellationToken token);
    }
}
