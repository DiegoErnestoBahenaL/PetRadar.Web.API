using Microsoft.AspNetCore.Http;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IAdoptionAnimalDomain
    {
        Task<List<AdoptionAnimalEntity>> GetAllAsync(CancellationToken token);
        Task<List<AdoptionAnimalEntity>> GetAllByShelterIdAsync(long shelterId, CancellationToken token);
        string? GetMainPicturePath(AdoptionAnimalEntity animalDb, CancellationToken token);
        Task<AdoptionAnimalEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<AdoptionAnimalEntity> CreateAsync(AdoptionAnimalCreateModel animal, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(AdoptionAnimalEntity animalDb, AdoptionAnimalUpdateModel animal, long modifiedByUserId, CancellationToken token);
        Task<int> UpdateMainPictureAsync(AdoptionAnimalEntity animalDb, IFormFile file, long modifiedByUserId, CancellationToken token);
        List<string> GetAdditionalPhotoNames(AdoptionAnimalEntity animalDb);
        string? GetAdditionalPhotoPath(string relativePath, string imageName);
        Task<int> UploadAdditionalPhotosAsync(AdoptionAnimalEntity animalDb, List<IFormFile> files, string? guid, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAdditionalPhotoAsync(AdoptionAnimalEntity animalDb, string photoName, long modifiedByUserId, CancellationToken token);

        Task<int> DeleteAsync(AdoptionAnimalEntity animal, long modifiedByUserId, CancellationToken token);
    }
}
