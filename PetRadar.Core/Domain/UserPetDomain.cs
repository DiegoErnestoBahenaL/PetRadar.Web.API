using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetRadar.Common;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class UserPetDomain : IUserPetDomain
    {
        private readonly IUserPetRepository _repo;
        private readonly IFileHelperService _fileHelperService;
        private readonly ILogger<UserPetDomain> _logger;

        public UserPetDomain(IUserPetRepository repo, IFileHelperService fileHelperService, ILogger<UserPetDomain> logger)
        {
            _repo = repo;
            _fileHelperService = fileHelperService;
            _logger = logger;
        }

        public Task<List<UserPetEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public Task<List<UserPetEntity>> GetAllByUserIdAsync(long userId, CancellationToken token)
        {
            return _repo.GetAllByUserIdAsync(userId, token);
        }

        public async Task<UserPetEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var pet = await _repo.FindByIdAsync(id, token);
            if (pet == null)
                return default;

            return pet;
        }
        public Task<string?> GetMainPicturePath(UserPetEntity petdb, CancellationToken token)
        {
            if (petdb.PhotoURL == null)
                return Task.FromResult<string?>(null);

            string path = _fileHelperService.GetImagePath(petdb.PhotoURL);
            return Task.FromResult<string?>(path);
        }


        public async Task<UserPetEntity> CreateAsync(UserPetCreateModel pet, long createdByUserId, CancellationToken token)
        {
            var petdb = new UserPetEntity(
                pet.UserId.Value, pet.Name, pet.Species.Value, pet.Breed, pet.Color,
                pet.Sex, pet.Size, pet.BirthDate, pet.ApproximateAge,
                pet.Weight, pet.Description, pet.IsNeutered, pet.Allergies, pet.MedicalNotes
            );

            petdb.CreatedBy = createdByUserId;
            petdb.CreatedAt = petdb.UpdatedAt = DateTime.UtcNow;
            petdb.IsActive = true;

            await _repo.AddAsync(petdb);
            await _repo.SaveChangesAsync();
            return petdb;
        }

        public async Task<int> UpdateAsync(UserPetEntity petdb, UserPetUpdateModel pet, long modifiedByUserId, CancellationToken token)
        {
            if (petdb == default)
                throw new ArgumentNullException(nameof(petdb));

            if (!string.IsNullOrEmpty(pet.Name))
                petdb.Name = pet.Name;

            if (pet.Species.HasValue)
                petdb.Species = pet.Species.Value;

            if (!string.IsNullOrEmpty(pet.Breed))
                petdb.Breed = pet.Breed;

            if (!string.IsNullOrEmpty(pet.Color))
                petdb.Color = pet.Color;

            if (pet.Sex.HasValue)
                petdb.Sex = pet.Sex.Value;

            if (pet.Size.HasValue)
                petdb.Size = pet.Size.Value;

            if (pet.BirthDate.HasValue)
                petdb.BirthDate = pet.BirthDate.Value;

            if (pet.ApproximateAge.HasValue)
                petdb.ApproximateAge = pet.ApproximateAge.Value;

            if (pet.Weight.HasValue)
                petdb.Weight = pet.Weight.Value;

            if (!string.IsNullOrEmpty(pet.Description))
                petdb.Description = pet.Description;

            if (pet.IsNeutered.HasValue)
                petdb.IsNeutered = pet.IsNeutered.Value;

            if (!string.IsNullOrEmpty(pet.Allergies))
                petdb.Allergies = pet.Allergies;

            if (!string.IsNullOrEmpty(pet.MedicalNotes))
                petdb.MedicalNotes = pet.MedicalNotes;

            petdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(petdb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> UpdateMainPictureAsync(UserPetEntity petdb, IFormFile file, long modifiedByUserId, CancellationToken token)
        {

            if (petdb.PhotoURL != null)
            {
                _fileHelperService.DeleteImage(petdb.PhotoURL, _logger);

                petdb.PhotoURL = null;
            }

            string relativePath = await _fileHelperService.SaveImage(file);

            petdb.PhotoURL = relativePath;

            petdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(petdb);

            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public List<string> GetAdditionalPhotoNames(UserPetEntity petdb)
        {
            if (petdb.AdditionalPhotosURL == null)
                return [];

            List<string> paths = _fileHelperService.GetGalleryImageNames(petdb.AdditionalPhotosURL);
            return paths;
        }

        public string? GetAdditionalPhotoPath(string relativePath, string imageName)
        {

            string imageRelativePath = Path.Combine(relativePath, imageName);

            string path = string.Empty;

            try 
            {
                path = _fileHelperService.GetImagePath(imageRelativePath);
            }
            catch (PetRadarException ex)
            {
                _logger.LogError(ex, "Error retrieving additional photo at path: {ImageRelativePath}", imageRelativePath);
                return null;
            }

            return path;
        }

        public async Task<int> UploadAdditionalPhotosAsync (UserPetEntity petdb, List<IFormFile> files, string? guid, long modifiedByUserId, CancellationToken token)
        {

            string? relativePath = await _fileHelperService.SaveImagesInGallery(files, guid);

            if (relativePath != null)
            {
                petdb.AdditionalPhotosURL = relativePath;
            }

            petdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(petdb);
            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public async Task<int> DeleteAsync(UserPetEntity pet, long modifiedByUserId, CancellationToken token)
        {
            if (pet == default)
                throw new ArgumentNullException(nameof(pet));

            pet.IsActive = false;

            pet.DeletedByUser(modifiedByUserId);
            _repo.Update(pet);

            return await _repo.SaveChangesAsync();
        }
    }
}
