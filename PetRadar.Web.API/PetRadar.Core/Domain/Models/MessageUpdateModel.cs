using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class MessageUpdateModel
    {
        public MessageUpdateModel() { }

        [StringLength(maximumLength: 5000, MinimumLength = 1, ErrorMessage = "The field Content must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string? Content { get; set; }

        public bool? Read { get; set; }

        public DateTimeOffset? ReadDate { get; set; }

        public MessageUpdateModel(string? content, bool? read, DateTimeOffset? readDate)
        {
            Content = content;
            Read = read;
            ReadDate = readDate;
        }
    }
}
