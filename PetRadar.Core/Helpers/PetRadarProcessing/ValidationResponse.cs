using Newtonsoft.Json;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public class ValidationResponse
    {
        [JsonProperty("detectedClass")]
        public string DetectedClass { get; set; }

        [JsonProperty("confidence")]
        public float Confidence { get; set; }
    }
}
