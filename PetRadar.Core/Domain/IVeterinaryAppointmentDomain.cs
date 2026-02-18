using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IVeterinaryAppointmentDomain
    {
        Task<List<VeterinaryAppointmentEntity>> GetAllAsync(CancellationToken token);
        Task<List<VeterinaryAppointmentEntity>> GetAllByPetIdAsync(long petId, CancellationToken token);
        Task<List<VeterinaryAppointmentEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<VeterinaryAppointmentEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<VeterinaryAppointmentEntity> CreateAsync(VeterinaryAppointmentCreateModel appointment, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(VeterinaryAppointmentEntity appointmentDb, VeterinaryAppointmentUpdateModel appointment, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(VeterinaryAppointmentEntity appointment, long modifiedByUserId, CancellationToken token);
    }
}
