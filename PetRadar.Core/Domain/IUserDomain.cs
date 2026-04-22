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
    public interface IUserDomain
    {
        Task<List<UserEntity>> GetAllAsync(CancellationToken token);
        string? GetUserProfilePicturePath(UserEntity user, CancellationToken token);
        Task<UserEntity?> FindByIdAsync(long id, CancellationToken token);
        Task<UserEntity?> FindByEmailAsync(string email, CancellationToken token);
        Task<UserEntity?> FindByEmailAndPasswordAsync(string email, string password, CancellationToken token);
        Task<UserEntity> CreateAsync(UserCreateModel user, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(UserEntity userdb, UserUpdateModel user, long modifiedByUserId, CancellationToken token);
        Task<int> VerifyEmailAsync(UserEntity userdb, long modifiedByUserId);
        Task<int> UpdateProfilePictureAsync(UserEntity userdb, IFormFile file, long modifiedByUserId, CancellationToken token);
        Task<int> UpdateFcmTokenAsync(UserEntity userdb, string? fcmToken, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(UserEntity user, long modifiedByUserId, CancellationToken token);
    }
}
