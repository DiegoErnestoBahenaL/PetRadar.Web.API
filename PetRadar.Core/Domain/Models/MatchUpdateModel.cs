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
    public class MatchUpdateModel
    {
        public MatchUpdateModel() { }

        public double? Score { get; set; }

        public double? DistanceInKM { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MatchStatusEnum? Status { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Notes must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Notes { get; set; }

        public DateTimeOffset? ConfirmationDate { get; set; }

        public MatchUpdateModel(
            double? score, double? distanceInKM, MatchStatusEnum? status,
            string? notes, DateTimeOffset? confirmationDate)
        {
            Score = score;
            DistanceInKM = distanceInKM;
            Status = status;
            Notes = notes;
            ConfirmationDate = confirmationDate;
        }
    }
}
