using PetRadar.Core.Data.Entities.Enums;
using System.IO;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public interface IPetRadarProcessingHelperService
    {
        Task<ValidationResponse> ValidateCatOrDogAsync(Stream imageStream, string fileName, string contentType);
        Task<CharacteristicsResponse> GetAnimalCharacteristicsAsync(PetSpeciesEnum species, Stream imageStream, string fileName, string contentType);
        Task<ConfigsResponse> GetConfigs();
        Task<UpdateConfigsResponse> UpdateConfigs(string yoloConfThreshold, string topKBreedPrediction);
    }
}
