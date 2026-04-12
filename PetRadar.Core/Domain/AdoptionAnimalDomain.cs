using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using PetRadar.Core.Helpers.PetRadarProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class AdoptionAnimalDomain : IAdoptionAnimalDomain
    {
        private readonly IAdoptionAnimalRepository _repo;
        private readonly IFileHelperService _fileHelperService;
        private readonly ILogger<AdoptionAnimalDomain> _logger;
        private readonly IPetRadarProcessingHelperService _processingHelperService;

        public AdoptionAnimalDomain(IAdoptionAnimalRepository repo, IFileHelperService fileHelperService, ILogger<AdoptionAnimalDomain> logger, IPetRadarProcessingHelperService processingHelperService)
        {
            _repo = repo;
            _fileHelperService = fileHelperService;
            _logger = logger;
            _processingHelperService = processingHelperService;
        }

        public Task<List<AdoptionAnimalEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<AdoptionAnimalEntity>> GetAllByShelterIdAsync(long shelterId, CancellationToken token)
        {
            return _repo.GetAllByShelterIdAsync(shelterId, token);
        }

        public async Task<AdoptionAnimalEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var animal = await _repo.FindByIdAsync(id, token);
            if (animal == null)
                return default;

            animal.Views++;

            _repo.Update(animal);

            await _repo.SaveChangesAsync();

            return animal;
        }

        public Task<string?> GetMainPicturePath(AdoptionAnimalEntity animalDb, CancellationToken token)
        {
            if (animalDb.PhotoURL == null)
                return Task.FromResult<string?>(null);

            string path = _fileHelperService.GetImagePath(animalDb.PhotoURL);
            return Task.FromResult<string?>(path);
        }

        public async Task<AdoptionAnimalEntity> CreateAsync(AdoptionAnimalCreateModel animal, long createdByUserId, CancellationToken token)
        {
            var animalDb = new AdoptionAnimalEntity(
                animal.ShelterId.Value, animal.Name, animal.Species.Value, animal.Breed, animal.Color,
                animal.Sex, animal.Size, animal.ApproximateAge,
                animal.Weight, animal.Description, animal.IsNeutered,
                animal.Personality, animal.GoodWithKids, animal.GoodWithDogs,
                animal.GoodWithCats, animal.IsVaccinated, animal.NeedsSpecialCare, animal.SpecialCareDetails
            );

            animalDb.CreatedBy = createdByUserId;
            animalDb.CreatedAt = animalDb.UpdatedAt = DateTime.UtcNow;
            animalDb.IsActive = true;

            await _repo.AddAsync(animalDb);
            await _repo.SaveChangesAsync();
            return animalDb;
        }

        public async Task<int> UpdateAsync(AdoptionAnimalEntity animalDb, AdoptionAnimalUpdateModel animal, long modifiedByUserId, CancellationToken token)
        {
            if (animalDb == default)
                throw new ArgumentNullException(nameof(animalDb));

            if (!string.IsNullOrEmpty(animal.Name))
                animalDb.Name = animal.Name;

            if (animal.Species.HasValue)
                animalDb.Species = animal.Species.Value;

            if (!string.IsNullOrEmpty(animal.Breed))
                animalDb.Breed = animal.Breed;

            if (!string.IsNullOrEmpty(animal.Color))
                animalDb.Color = animal.Color;

            if (animal.Sex.HasValue)
                animalDb.Sex = animal.Sex.Value;

            if (animal.Size.HasValue)
                animalDb.Size = animal.Size.Value;

            if (animal.ApproximateAge.HasValue)
                animalDb.ApproximateAge = animal.ApproximateAge.Value;

            if (animal.Weight.HasValue)
                animalDb.Weight = animal.Weight.Value;

            if (!string.IsNullOrEmpty(animal.Description))
                animalDb.Description = animal.Description;

            if (animal.IsNeutered.HasValue)
                animalDb.IsNeutered = animal.IsNeutered.Value;

            if (!string.IsNullOrEmpty(animal.Personality))
                animalDb.Personality = animal.Personality;

            if (animal.GoodWithKids.HasValue)
                animalDb.GoodWithKids = animal.GoodWithKids.Value;

            if (animal.GoodWithDogs.HasValue)
                animalDb.GoodWithDogs = animal.GoodWithDogs.Value;

            if (animal.GoodWithCats.HasValue)
                animalDb.GoodWithCats = animal.GoodWithCats.Value;

            if (animal.IsVaccinated.HasValue)
                animalDb.IsVaccinated = animal.IsVaccinated.Value;

            if (animal.NeedsSpecialCare.HasValue)
                animalDb.NeedsSpecialCare = animal.NeedsSpecialCare.Value;

            if (!string.IsNullOrEmpty(animal.SpecialCareDetails))
                animalDb.SpecialCareDetails = animal.SpecialCareDetails;

            if (animal.Status.HasValue)
                animalDb.Status = animal.Status.Value;

            if (animal.AdoptionDate.HasValue)
                animalDb.AdoptionDate = animal.AdoptionDate.Value;

            if (animal.AdopterId.HasValue)
                animalDb.AdopterId = animal.AdopterId.Value;

            animalDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(animalDb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> UpdateMainPictureAsync(AdoptionAnimalEntity animalDb, IFormFile file, long modifiedByUserId, CancellationToken token)
        {
            await using var imageStream = file.OpenReadStream();

            try
            {
                var validationResult = await _processingHelperService.ValidateCatOrDogAsync(imageStream, file.FileName, file.ContentType);

                if (animalDb.Species != Data.Entities.Enums.PetSpeciesEnum.NotSet && 
                    animalDb.Species.ToString().ToLower() != validationResult.DetectedClass.ToLower())
                {
                    throw new BadHttpRequestException("The image detected class is different from the registered species.");
                }
            }
            catch (BadHttpRequestException ex) 
            { 
                throw new BadHttpRequestException($"Image validation failed: {ex.Message}");
            }

            if (animalDb.PhotoURL != null)
            {
                _fileHelperService.DeleteImage(animalDb.PhotoURL, _logger);

                animalDb.PhotoURL = null;
            }

            string relativePath = await _fileHelperService.SaveImage(file);

            animalDb.PhotoURL = relativePath;

            animalDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(animalDb);

            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public List<string> GetAdditionalPhotoNames(AdoptionAnimalEntity animalDb)
        {
            if (animalDb.AdditionalPhotosURL == null)
                return [];

            return _fileHelperService.GetGalleryImageNames(animalDb.AdditionalPhotosURL);
        }

        public string? GetAdditionalPhotoPath(string relativePath, string imageName)
        {
            // Delegates to the file helper which validates the filename (blocks path traversal)
            // and verifies the image exists on disk.
            return _fileHelperService.GetGalleryImagePath(relativePath, imageName);
        }

        public async Task<int> UploadAdditionalPhotosAsync(AdoptionAnimalEntity animalDb, List<IFormFile> files, string? guid, long modifiedByUserId, CancellationToken token)
        {
            try
            {
                foreach (var file in files)
                {
                    await using var imageStream = file.OpenReadStream();

                    var validationResult = await _processingHelperService.ValidateCatOrDogAsync(imageStream, file.FileName, file.ContentType);

                    if (animalDb.Species != Data.Entities.Enums.PetSpeciesEnum.NotSet &&
                        animalDb.Species.ToString().ToLower() != validationResult.DetectedClass.ToLower())
                    {
                        throw new BadHttpRequestException("The image detected class is different from the registered species.");
                    }
                }
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException($"Image validation failed: {ex.Message}");
            }

            string? relativePath = await _fileHelperService.SaveImagesInGallery(files, guid);

            if (relativePath != null)
            {
                animalDb.AdditionalPhotosURL = relativePath;
            }

            animalDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(animalDb);
            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public async Task<int> DeleteAdditionalPhotoAsync(AdoptionAnimalEntity animalDb, string photoName, long modifiedByUserId, CancellationToken token)
        {
            if (animalDb == default)
                throw new ArgumentNullException(nameof(animalDb));

            if (string.IsNullOrEmpty(animalDb.AdditionalPhotosURL))
                throw new InvalidOperationException("Adoption animal has no additional photos gallery.");

            var path = GetAdditionalPhotoPath(animalDb.AdditionalPhotosURL, photoName);
            if (path == null)
                throw new FileNotFoundException($"Additional photo '{photoName}' not found.");

            File.Delete(path);

            animalDb.UpdatedByUser(modifiedByUserId);
            _repo.Update(animalDb);
            return await _repo.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(AdoptionAnimalEntity animal, long modifiedByUserId, CancellationToken token)
        {
            if (animal == default)
                throw new ArgumentNullException(nameof(animal));

            animal.IsActive = false;

            animal.DeletedByUser(modifiedByUserId);

            if (animal.PhotoURL != null)
            {
                _fileHelperService.DeleteImage(animal.PhotoURL, _logger);
                animal.PhotoURL = null;
            }

            if (!string.IsNullOrEmpty(animal.AdditionalPhotosURL))
            {
                var additionalPhotoNames = GetAdditionalPhotoNames(animal);
                foreach (var photoName in additionalPhotoNames)
                {
                    var path = GetAdditionalPhotoPath(animal.AdditionalPhotosURL, photoName);
                    if (path != null)
                    {
                        File.Delete(path);
                    }
                }
                animal.AdditionalPhotosURL = null;
            }

            _repo.Update(animal);

            return await _repo.SaveChangesAsync();
        }
    }
}
