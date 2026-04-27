using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain.Models
{
    public class UpdateSystemConfigsModel
    {
        public float? YoloConfThreshold { get; set; }
        public int? TopKBreedPredictions { get; set; }
        public decimal? TopKBreedPredictionThreshold { get; set; }

        public UpdateSystemConfigsModel() { }
    
        public UpdateSystemConfigsModel(float? yoloConfThreshold, int? topKBreedPredictions, decimal? topKBreedPredictionThreshold)
        {
            YoloConfThreshold = yoloConfThreshold;
            TopKBreedPredictions = topKBreedPredictions;
            TopKBreedPredictionThreshold = topKBreedPredictionThreshold;
        }
    }
}
