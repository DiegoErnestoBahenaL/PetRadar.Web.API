using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class UpdateConfigsResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } 
    }
}
