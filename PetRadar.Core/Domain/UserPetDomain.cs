using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
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

        public UserPetDomain(IUserPetRepository repo)
        {
            _repo = repo;
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

        public async Task<UserPetEntity> CreateAsync(UserPetCreateModel pet, long createdByUserId, CancellationToken token)
        {
            var petdb = new UserPetEntity(
                pet.UserId, pet.Name, pet.Species, pet.Breed, pet.Color,
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
