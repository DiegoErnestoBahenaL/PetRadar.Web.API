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
    public class NotificationEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]       
        public UserEntity User { get; set; }
        
        [Required]
        public NotificationTypeEnum Type { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string? Metadata { get; set; }

        [StringLength(maximumLength: 500)]
        public string? DeepLink { get; set; }
        public bool Read { get; set; } = false;
        public DateTimeOffset? ReadDate { get; set; }

        public NotificationEntity() { }

        public NotificationEntity
        (
            long userId, NotificationTypeEnum type, string title, string message,
            string? metadata, string? deepLink, bool read, DateTimeOffset? readDate
        )
        {
            UserId = userId;
            Type = type;
            Title = title;
            Message = message;
            Metadata = metadata;
            DeepLink = deepLink;
            Read = read;
            ReadDate = readDate;
        }
    }
}
