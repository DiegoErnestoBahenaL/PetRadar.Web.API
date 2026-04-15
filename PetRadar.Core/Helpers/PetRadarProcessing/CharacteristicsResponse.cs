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
        public string TopPredictedBreed { get; set; }

        [JsonProperty("colors")]
        public List<ColorExtracted> Colors { get; set; } = new List<ColorExtracted>();

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

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

    public class ColorExtracted
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("proportion")]
        public decimal Proportion { get; set; } = 0M;
    }

}
