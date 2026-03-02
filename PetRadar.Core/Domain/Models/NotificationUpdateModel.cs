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
    public class NotificationUpdateModel
    {
        public NotificationUpdateModel() { }

        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationTypeEnum? Type { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "The field Title must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Title { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field Message must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Message { get; set; }

        public string? Metadata { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "The field DeepLink must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? DeepLink { get; set; }

        public bool? Read { get; set; }

        public DateTimeOffset? ReadDate { get; set; }

        public NotificationUpdateModel(
            NotificationTypeEnum? type, string? title, string? message,
            string? metadata, string? deepLink, bool? read, DateTimeOffset? readDate)
        {
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
