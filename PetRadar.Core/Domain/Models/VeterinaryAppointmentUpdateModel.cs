using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PetRadar.Core.Data.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class VeterinaryAppointmentUpdateModel
    {
        public VeterinaryAppointmentUpdateModel() { }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field VeterinaryName must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? VeterinaryName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentTypeEnum? AppointmentType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentStatusEnum? AppointmentStatus { get; set; }

        public DateTimeOffset? AppointmentDate { get; set; }

        public int? DurationInMinutes { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field ReasonForVisit must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? ReasonForVisit { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Notes must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Notes { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Diagnosis must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Diagnosis { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Treatment must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Treatment { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Prescriptions must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Prescriptions { get; set; }

        public decimal? Cost { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [StringLength(200, MinimumLength = 1, ErrorMessage = "The field AddressText must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? AddressText { get; set; }

        public VeterinaryAppointmentUpdateModel(
            string? veterinaryName, AppointmentTypeEnum? appointmentType,
            AppointmentStatusEnum? appointmentStatus, DateTimeOffset? appointmentDate, int? durationInMinutes,
            string? reasonForVisit, string? notes, string? diagnosis, string? treatment,
            string? prescriptions, decimal? cost, string? addressText,
            double? latitude = null, double? longitude = null)
        {
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
            AddressText = addressText;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
