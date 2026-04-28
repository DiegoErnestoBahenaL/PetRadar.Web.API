using PetRadar.Core.Helpers.PetRadarProcessing;

namespace PetRadar.Web.API.ViewModels
{
    public class ConfigsViewModel
    {
        public float YoloConfThreshold { get; set; }
        public int TopKBreedPredictions { get; set; }
        public decimal TopKBreedPredictionThreshold { get; set; }

        public ConfigsViewModel() { }

        public ConfigsViewModel(ConfigsResponse configsResponse, string? topKBreedPredictionThreshold)
        {
            YoloConfThreshold = configsResponse.YoloConfThreshold;
            TopKBreedPredictions = configsResponse.TopKBreedPredictions;
            TopKBreedPredictionThreshold = decimal.Parse(topKBreedPredictionThreshold ?? "0");
        }

    }   
}
