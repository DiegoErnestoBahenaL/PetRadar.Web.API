using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class VeterinaryAppointmentDomain : IVeterinaryAppointmentDomain
    {
        private readonly IVeterinaryAppointmentRepository _repo;

        public VeterinaryAppointmentDomain(IVeterinaryAppointmentRepository repo)
        {
            _repo = repo;
        }

        public Task<List<VeterinaryAppointmentEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<VeterinaryAppointmentEntity>> GetAllByPetIdAsync(long petId, CancellationToken token)
        {
            return _repo.GetAllByPetIdAsync(petId, token);
        }

        public Task<List<VeterinaryAppointmentEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            return _repo.GetAllByUserIdAsync(userId, token);
        }

        public async Task<VeterinaryAppointmentEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var appointment = await _repo.FindByIdAsync(id, token);
            if (appointment == null)
                return default;

            return appointment;
        }

        public async Task<VeterinaryAppointmentEntity> CreateAsync(VeterinaryAppointmentCreateModel appointment, long createdByUserId, CancellationToken token)
        {



            var appointmentDb = new VeterinaryAppointmentEntity (
                appointment.PetId, appointment.VeterinaryName, appointment.AppointmentType,
                appointment.AppointmentStatus, appointment.AppointmentDate, appointment.DurationInMinutes,
                appointment.ReasonForVisit, appointment.Notes, appointment.Diagnosis, appointment.Treatment, 
                appointment.Prescriptions, appointment.Cost, appointment.Location, appointment.AddressText
            );

            appointmentDb.CreatedBy = createdByUserId;
            appointmentDb.CreatedAt = appointmentDb.UpdatedAt = DateTime.UtcNow;
            appointmentDb.IsActive = true;

            await _repo.AddAsync(appointmentDb);
            await _repo.SaveChangesAsync();
            return appointmentDb;
        }

        public async Task<int> UpdateAsync(VeterinaryAppointmentEntity appointmentDb, VeterinaryAppointmentUpdateModel appointment, long modifiedByUserId, CancellationToken token)
        {
            if (appointmentDb == default)
                throw new ArgumentNullException(nameof(appointmentDb));

            if (!string.IsNullOrEmpty(appointment.VeterinaryName))
                appointmentDb.VeterinaryName = appointment.VeterinaryName;

            if (appointment.AppointmentType.HasValue)
                appointmentDb.AppointmentType = appointment.AppointmentType.Value;

            if (appointment.AppointmentStatus.HasValue)
                appointmentDb.AppointmentStatus = appointment.AppointmentStatus.Value;

            if (appointment.AppointmentDate.HasValue)
                appointmentDb.AppointmentDate = appointment.AppointmentDate.Value;

            if (appointment.DurationInMinutes.HasValue)
                appointmentDb.DurationInMinutes = appointment.DurationInMinutes.Value;

            if (!string.IsNullOrEmpty(appointment.ReasonForVisit))
                appointmentDb.ReasonForVisit = appointment.ReasonForVisit;

            if (!string.IsNullOrEmpty(appointment.Notes))
                appointmentDb.Notes = appointment.Notes;

            if (!string.IsNullOrEmpty(appointment.Diagnosis))
                appointmentDb.Diagnosis = appointment.Diagnosis;

            if (!string.IsNullOrEmpty(appointment.Treatment))
                appointmentDb.Treatment = appointment.Treatment;

            if (!string.IsNullOrEmpty(appointment.Prescriptions))
                appointmentDb.Prescriptions = appointment.Prescriptions;

            if (appointment.Cost.HasValue)
                appointmentDb.Cost = appointment.Cost.Value;

            if (!string.IsNullOrEmpty(appointment.AddressText))
                appointmentDb.AddressText = appointment.AddressText;

            appointmentDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(appointmentDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteAsync(VeterinaryAppointmentEntity appointment, long modifiedByUserId, CancellationToken token)
        {
            if (appointment == default)
                throw new ArgumentNullException(nameof(appointment));

            appointment.IsActive = false;

            appointment.DeletedByUser(modifiedByUserId);
            _repo.Update(appointment);

            return await _repo.SaveChangesAsync();
        }
    }
}
