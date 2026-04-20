using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class MessageCreateModel
    {
        public MessageCreateModel() { }

        [Required]
        public long SenderId { get; set; }

        [Required]
        public long RecipientId { get; set; }

        [Required]
        [StringLength(maximumLength: 5000, MinimumLength = 1, ErrorMessage = "The field Content must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string Content { get; set; } = string.Empty;

        public long? MatchId { get; set; }

        public long? AdoptionAnimalId { get; set; }

        public MessageCreateModel(
            long senderId, long recipientId, string content,
            long? matchId, long? adoptionAnimalId)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Content = content;
            MatchId = matchId;
            AdoptionAnimalId = adoptionAnimalId;
        }
    }
}
