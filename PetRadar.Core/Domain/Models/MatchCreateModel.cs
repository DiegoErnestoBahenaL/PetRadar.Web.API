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
    public class MatchCreateModel
    {
        public MatchCreateModel() { }

        [Required]
        public long LostReportId { get; set; }

        [Required]
        public long StrayReportId { get; set; }

        public double? DistanceInKM { get; set; }

        [StringLength(500, ErrorMessage = "The field Notes must be a string with maximum length of {1}")]
        public string? Notes { get; set; }

        public DateTimeOffset? ConfirmationDate { get; set; }

        public MatchCreateModel(
            long lostReportId, long strayReportId, double? distanceInKM,
            string? notes, DateTimeOffset? confirmationDate)
        {
            LostReportId = lostReportId;
            StrayReportId = strayReportId;
            DistanceInKM = distanceInKM;
            Notes = notes;
            ConfirmationDate = confirmationDate;
        }
    }
}
