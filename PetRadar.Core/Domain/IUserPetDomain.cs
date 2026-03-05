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
    public interface IUserPetDomain
    {
        Task<List<UserPetEntity>> GetAllAsync(CancellationToken token);
        Task<List<UserPetEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<string?> GetMainPicturePath(UserPetEntity petdb, CancellationToken token);
        Task<UserPetEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<UserPetEntity> CreateAsync(UserPetCreateModel pet, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(UserPetEntity petdb, UserPetUpdateModel pet, long modifiedByUserId, CancellationToken token);
        Task<int> UpdateMainPictureAsync(UserPetEntity petdb, IFormFile file, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(UserPetEntity pet, long modifiedByUserId, CancellationToken token);
    }
}
