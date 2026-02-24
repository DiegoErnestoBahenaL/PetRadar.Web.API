using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class AdoptionAnimalRepository : EntityRepository<AdoptionAnimalEntity>, IAdoptionAnimalRepository
    {
        public AdoptionAnimalRepository(PetRadarDbContext db) : base(db, db.AdoptionAnimals) { }

        public Task<List<AdoptionAnimalEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<AdoptionAnimalEntity>> GetAllByShelterIdAsync(long shelterId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.ShelterId == shelterId);

            return query.ToListAsync(token);
        }

        public Task<AdoptionAnimalEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.AdoptionAnimals
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
