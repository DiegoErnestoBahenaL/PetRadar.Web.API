using Microsoft.EntityFrameworkCore;
using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public class VeterinaryAppointmentRepository : EntityRepository<VeterinaryAppointmentEntity>, IVeterinaryAppointmentRepository
    {
        public VeterinaryAppointmentRepository(PetRadarDbContext db) : base(db, db.VeterinaryAppointments) { }

        public Task<List<VeterinaryAppointmentEntity>> GetAllAsync(CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true);

            return query.ToListAsync(token);
        }

        public Task<List<VeterinaryAppointmentEntity>> GetAllByPetIdAsync(long petId, CancellationToken token)
        {
            var query = ConstructQuery()
                .Where(x => x.IsActive == true && x.PetId == petId);

            return query.ToListAsync(token);
        }

        public Task<List<VeterinaryAppointmentEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {

            return _dbContext.VeterinaryAppointments
                .Include(x => x.Pet)
                .Where(x => x.IsActive == true && x.Pet.UserId == userId)
                .ToListAsync(token);
        }

        public Task<VeterinaryAppointmentEntity?> FindByIdAsync(long id, CancellationToken token)
        {
            return _dbContext.VeterinaryAppointments
                .Where(x => x.IsActive == true)
                .SingleOrDefaultAsync(x => x.Id == id, token);
        }
    }
}
