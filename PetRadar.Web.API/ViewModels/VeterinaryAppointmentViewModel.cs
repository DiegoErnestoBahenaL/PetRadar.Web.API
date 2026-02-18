using PetRadar.Core.Data.Entities;

namespace PetRadar.Web.API.ViewModels
{
    public class VeterinaryAppointmentViewModel
    {
        public long Id { get; set; }
        public long PetId { get; set; }
        public string VeterinaryName { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
        public string AppointmentStatus { get; set; } = string.Empty;
        public DateTimeOffset AppointmentDate { get; set; }
        public int? DurationInMinutes { get; set; }
        public string ReasonForVisit { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Prescriptions { get; set; }
        public decimal? Cost { get; set; }
        public string? AddressText { get; set; }
        public bool ReminderSent { get; set; }

        public VeterinaryAppointmentViewModel() { }

        public VeterinaryAppointmentViewModel(VeterinaryAppointmentEntity entity)
        {
            Id = entity.Id;
            PetId = entity.PetId;
            VeterinaryName = entity.VeterinaryName;
            AppointmentType = entity.AppointmentType.ToString();
            AppointmentStatus = entity.AppointmentStatus.ToString();
            AppointmentDate = entity.AppointmentDate;
            DurationInMinutes = entity.DurationInMinutes;
            ReasonForVisit = entity.ReasonForVisit;
            Notes = entity.Notes;
            Diagnosis = entity.Diagnosis;
            Treatment = entity.Treatment;
            Prescriptions = entity.Prescriptions;
            Cost = entity.Cost;
            AddressText = entity.AddressText;
            ReminderSent = entity.ReminderSent;
        }

        public static List<VeterinaryAppointmentViewModel> FromList(List<VeterinaryAppointmentEntity> entities)
        {
            var viewModels = new List<VeterinaryAppointmentViewModel>();
            foreach (var entity in entities)
            {
                viewModels.Add(new VeterinaryAppointmentViewModel(entity));
            }
            return viewModels;
        }
    }
}
