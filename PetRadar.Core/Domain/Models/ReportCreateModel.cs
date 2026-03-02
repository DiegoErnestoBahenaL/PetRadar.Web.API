using NetTopologySuite.Geometries;
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
    public class ReportCreateModel
    {
        public ReportCreateModel() { }

        [Required]
        public long? UserId { get; set; }

        public long? UserPetId { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public PetSpeciesEnum? Species { get; set; }

        [StringLength(100, ErrorMessage = "The field Breed must be a string with maximum length of {1}")]
        public string? Breed { get; set; }

        [StringLength(100, ErrorMessage = "The field Color must be a string with a maximum length of {1}")]
        public string? Color { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSexEnum? Sex { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PetSizeEnum? Size { get; set; }

        public decimal? ApproximateAge { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(500, ErrorMessage = "The field Description must be a string with maximum length of {1}")]
        public string? Description { get; set; }

        public bool? IsNeutered { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ReportTypeEnum? ReportType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReportStatusEnum ReportStatus { get; set; } = ReportStatusEnum.Active;

        public bool? HasCollar { get; set; }

        public bool? HasTag { get; set; }

        public DateTimeOffset? IncidentDate { get; set; }

        [Required]
        public double? Latitude { get; set; }

        [Required]
        public double? Longitude { get; set; }

        [StringLength(500, ErrorMessage = "The field AddressText must be a string with maximum length of {1}")]
        public string? AddressText { get; set; }

        public int SearchRadiusMeters { get; set; } = 5000;

        public bool UseAlternateContact { get; set; } = false;

        [StringLength(100, ErrorMessage = "The field ContactName must be a string with maximum length of {1}")]
        public string? ContactName { get; set; }

        [StringLength(20, ErrorMessage = "The field ContactPhone must be a string with maximum length of {1}")]
        public string? ContactPhone { get; set; }

        [StringLength(255, ErrorMessage = "The field ContactEmail must be a string with maximum length of {1}")]
        public string? ContactEmail { get; set; }

        public bool OffersReward { get; set; } = false;

        public decimal? RewardAmount { get; set; }

        public ReportCreateModel(
            long userId, long? userPetId, PetSpeciesEnum species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, decimal? approximateAge, decimal? weight,
            string? description, bool? isNeutered, ReportTypeEnum reportType, ReportStatusEnum reportStatus,
            bool? hasCollar, bool? hasTag, DateTimeOffset? incidentDate,
            double latitude, double longitude, string? addressText, int searchRadiusMeters,
            bool useAlternateContact, string? contactName, string? contactPhone, string? contactEmail,
            bool offersReward, decimal? rewardAmount)
        {
            UserId = userId;
            UserPetId = userPetId;
            Species = species;
            Breed = breed;
            Color = color;
            Sex = sex;
            Size = size;
            ApproximateAge = approximateAge;
            Weight = weight;
            Description = description;
            IsNeutered = isNeutered;
            ReportType = reportType;
            ReportStatus = reportStatus;
            HasCollar = hasCollar;
            HasTag = hasTag;
            IncidentDate = incidentDate;
            Latitude = latitude;
            Longitude = longitude;
            AddressText = addressText;
            SearchRadiusMeters = searchRadiusMeters;
            UseAlternateContact = useAlternateContact;
            ContactName = contactName;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            OffersReward = offersReward;
            RewardAmount = rewardAmount;
        }
    }
}
