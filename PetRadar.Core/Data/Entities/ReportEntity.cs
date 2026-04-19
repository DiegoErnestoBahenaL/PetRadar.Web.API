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
    public class ReportEntity : PetEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

        public long? UserPetId { get; set; }

        [ForeignKey(nameof(UserPetId))]
        public UserPetEntity UserPet { get; set; }
        public ReportTypeEnum ReportType { get; set; }
        public ReportStatusEnum ReportStatus { get; set; } = ReportStatusEnum.Active;
        public bool? HasCollar { get; set; }
        public bool? HasTag { get; set; }

        [Required]
        public DateTimeOffset ReportDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? IncidentDate { get; set; }


        [Required, Column(TypeName = "geography")]
        public Point? Location { get; set; }

        [StringLength(maximumLength:500)]
        public string? AddressText { get; set; }
        public int SearchRadiusMeters { get; set; } = 5000;
        public bool UseAlternateContact { get; set; } = false;

        [StringLength(maximumLength: 100)]
        public string? ContactName { get; set; }

        [StringLength(maximumLength: 20)]
        public string? ContactPhone { get; set; }

        [StringLength(maximumLength: 255)]
        public string? ContactEmail { get; set; }

        public bool OffersReward { get; set; } = false;
        public decimal? RewardAmount { get; set; }
        public int Views { get; set; } = 0;
        [Column(TypeName = "jsonb")]
        public string? ImageAnalysisResult { get; set; }

        public ReportEntity() : base() { }


        public ReportEntity(long userId, long? userPetId, 
            PetSpeciesEnum species, string? breed, string? color,
            PetSexEnum? sex, PetSizeEnum? size, decimal? approximateAge,
            decimal? weight, string? description, bool? isNeutered, ReportTypeEnum reportType,
            ReportStatusEnum reportStatus, bool? hasCollar, bool? hasTag, DateTimeOffset? incidentDate,
            Point? location, string? addressText, bool useAlternateContact, string? contactName,
            string? contactPhone, string? contactEmail, decimal? rewardAmount)
            : base(species, breed, color, sex, size, approximateAge, weight, description, photoURL: null, additionalPhotosURL: null, isNeutered)
        {
            UserId = userId;
            UserPetId = userPetId;
            ReportType = reportType;
            ReportStatus = reportStatus;
            HasCollar = hasCollar;
            HasTag = hasTag;
            IncidentDate = incidentDate;
            Location = location;
            AddressText = addressText;
            UseAlternateContact = useAlternateContact;
            ContactName = contactName;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            RewardAmount = rewardAmount;

        }

    }
}
