using System.ComponentModel.DataAnnotations;

namespace PetRadar.Core.Domain.Models
{
    public class FcmTokenUpdateModel
    {
        [StringLength(4096)]
        public string? FcmToken { get; set; }
    }
}
