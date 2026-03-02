using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Data.Entities
{
    public class MessageEntity : Entity, IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long SenderId { get; set; }

        [ForeignKey(nameof(SenderId))]
        public UserEntity Sender { get; set; }

        [Required]
        public long RecipientId { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public UserEntity Recipient { get; set; }

        public long? ReportId { get; set; }

        [ForeignKey(nameof(ReportId))]
        public ReportEntity? Report { get; set; }

        public long? MatchId { get; set; }

        [ForeignKey(nameof(MatchId))]
        public MatchEntity? Match { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool Read { get; set; } = false;

        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ReadDate { get; set; }


        public MessageEntity() { }

        public MessageEntity(long senderId, long recipientId, string content, long? reportId = null, long? matchId = null)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Content = content;
            ReportId = reportId;
            MatchId = matchId;
        }

    }
}
