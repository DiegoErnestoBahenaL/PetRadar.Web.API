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
    public class NotificationCreateModel
    {
        public NotificationCreateModel() { }

        [Required]
        public long? UserId { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationTypeEnum? Type { get; set; }

        [Required, StringLength(255, MinimumLength = 1, ErrorMessage = "The field Title must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(500, MinimumLength = 1, ErrorMessage = "The field Message must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string Message { get; set; } = string.Empty;

        public string? Metadata { get; set; }

        [StringLength(500, ErrorMessage = "The field DeepLink must be a string with maximum length of {1}")]
        public string? DeepLink { get; set; }

        public NotificationCreateModel(
            long userId, NotificationTypeEnum type, string title, string message,
            string? metadata, string? deepLink)
        {
            UserId = userId;
            Type = type;
            Title = title;
            Message = message;
            Metadata = metadata;
            DeepLink = deepLink;
        }
    }
}
