using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class UserPetRepository : EntityRepository<UserPetEntity>, IUserPetRepository
    {
        public UserPetRepository(PetRadarDbContext db) : base(db, db.UserPets) { }

        public Task<List<UserPetEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<UserPetEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.UserId == userId);

            return query.ToListAsync(token);
        }

        public Task<UserPetEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.UserPets
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
