using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class ConfigsResponse
    {
        [JsonPropertyName("yoloConfThreshold")]
        public float YoloConfThreshold { get; set; }
        [JsonPropertyName("topKBreedPredictions")]
        public int TopKBreedPredictions { get; set; }
    }
}
