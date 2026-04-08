using System.IO;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers.PetRadarProcessing
{
    public interface IPetRadarProcessingHelperService
    {
        Task<ValidationResponse> ValidateCatOrDogAsync(Stream imageStream, string fileName, string contentType);
    }
}
