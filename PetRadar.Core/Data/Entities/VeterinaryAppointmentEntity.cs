using NetTopologySuite.Geometries;
using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public class VeterinaryAppointmentEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
       
        [Required]
        public long PetId { get; set; }
        
        [ForeignKey(nameof(PetId))]
        public UserPetEntity Pet { get; set; }
        
        [StringLength(maximumLength: 100)]
        public string VeterinaryName { get; set; } = string.Empty;

        [Required]
        public AppointmentTypeEnum AppointmentType { get; set; }

        [Required]
        public AppointmentStatusEnum AppointmentStatus { get; set; }

        [Required]
        public DateTimeOffset AppointmentDate { get; set; }

        public int? DurationInMinutes { get; set; }

        [Required, StringLength(maximumLength: 100)]
        public string ReasonForVisit { get; set; } = string.Empty;

        [StringLength(maximumLength: 500)]
        public string? Notes { get; set; }

        [StringLength(maximumLength: 500)]
        public string? Diagnosis { get; set; }

        [StringLength(maximumLength: 500)]
        public string? Treatment { get; set; }

        [StringLength(maximumLength: 500)]
        public string? Prescriptions { get; set; }

        public decimal? Cost { get; set; }

        [Column(TypeName = "geography")]
        public Point? Location { get; set; }

        [StringLength(maximumLength: 200)]
        public string? AddressText { get; set; }

        public bool ReminderSent { get; set; } = false; 

        public VeterinaryAppointmentEntity() { }

        public VeterinaryAppointmentEntity(long petId, string veterinaryName, 
            AppointmentTypeEnum appointmentType, AppointmentStatusEnum appointmentStatus, DateTimeOffset appointmentDate, 
            int? durationInMinutes, string reasonForVisit, string? notes, string? diagnosis, string? treatment, 
            string? prescriptions, decimal? cost, Point? location, string? addressText)
        {
            PetId = petId;
            VeterinaryName = veterinaryName;
            AppointmentType = appointmentType;
            AppointmentStatus = appointmentStatus;
            AppointmentDate = appointmentDate;
            DurationInMinutes = durationInMinutes;
            ReasonForVisit = reasonForVisit;
            Notes = notes;
            Diagnosis = diagnosis;
            Treatment = treatment;
            Prescriptions = prescriptions;
            Cost = cost;
            Location = location;
            AddressText = addressText;
           
        }
    }
}
