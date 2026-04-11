using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class CharacteristicsResponse
    {
        [JsonProperty("topPredictedBreed")]
        public string  TopPredictedBreed { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; } = 0M;

        [JsonProperty("topPredictions")]
        public List<TopPrediction> TopPredictions { get; set; } = new List<TopPrediction>();
    }

    public class TopPrediction
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("breed")]
        public string Breed { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; } = 0M;
    }
}
