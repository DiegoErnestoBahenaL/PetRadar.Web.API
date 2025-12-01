using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class UserRepository : EntityRepository<UserEntity>, IUserRepository
    {
        public UserRepository(PetRadarDbContext db) : base(db, db.Users) { }

        public Task<List<UserEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<UserEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.Users
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }

        public Task<UserEntity?> FindByEmailAsync(string email, CancellationToken token)
        {
            return _dbContext.Users
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync(x => x.Email == email, token);
        }
    }
}
