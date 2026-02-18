using PetRadar.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Repositories
{
    public interface IVeterinaryAppointmentRepository : IEntityRepository<VeterinaryAppointmentEntity>
    {
        Task<List<VeterinaryAppointmentEntity>> GetAllAsync(CancellationToken token);
        Task<List<VeterinaryAppointmentEntity>> GetAllByPetIdAsync(long petId, CancellationToken token);
        Task<List<VeterinaryAppointmentEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<VeterinaryAppointmentEntity?> FindByIdAsync(long id, CancellationToken token);
    }
}
