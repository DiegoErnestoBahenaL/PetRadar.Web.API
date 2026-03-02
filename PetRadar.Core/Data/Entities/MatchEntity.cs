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
    public class MatchEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long LostReportId { get; set; }

        [ForeignKey(nameof(LostReportId))]
        public ReportEntity LostReport { get; set; }

        [Required]
        public long StrayReportId { get; set; }

        [ForeignKey(nameof(StrayReportId))]
        public ReportEntity StrayReport { get; set; }

        [Required]
        public double Score { get; set; } = 0;

        public double? DistanceInKM { get; set; } = 0;

        [Required]
        public MatchStatusEnum Status { get; set; } = MatchStatusEnum.Pending;

        [StringLength(maximumLength: 500)]
        public string? Notes { get; set; }

        public DateTimeOffset? ConfirmationDate { get; set; }


        public MatchEntity() { }


        public MatchEntity 
        ( 
            long lostReportId, long strayReportId,double? distanceInKM, 
            string? notes, DateTimeOffset? confirmationDate
        )
        {
            LostReportId = lostReportId;
            StrayReportId = strayReportId;
            DistanceInKM = distanceInKM;
            Notes = notes;
            ConfirmationDate = confirmationDate;
        }
    }
}
