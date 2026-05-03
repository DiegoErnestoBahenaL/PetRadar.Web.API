using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using PetRadar.Common;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
using PetRadar.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public class UserDomain : IUserDomain
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IFileHelperService _fileHelperService; 
        private readonly IEmailHelperService _emailHelperService;
        private readonly ILogger<UserDomain> _logger;

        public UserDomain(IUserRepository repo, IPasswordHelper passwordHelper, IFileHelperService fileHelperService, ILogger<UserDomain> logger, IEmailHelperService emailHelperService) 
        {  
            _repo = repo; 
            _passwordHelper = passwordHelper;
            _fileHelperService = fileHelperService;
            _emailHelperService = emailHelperService;
            _logger = logger;
        }

        public Task<List<UserEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
        }

        public string? GetUserProfilePicturePath(UserEntity user, CancellationToken token)
        {
            if (user.ProfilePhotoURL == null)
                return null;

            try
            {
              return _fileHelperService.GetImagePath(user.ProfilePhotoURL);

            }
            catch (PetRadarException ex)
            {
                _logger.LogWarning(ex, "Profile picture not found for user {userId}: {message}", user.Id, ex.Message);
                return null;
            }
        }

        public async Task<UserEntity?> FindByIdAsync(long id, CancellationToken token = default)
        {
            var user = await _repo.FindByIdAsync(id, token);
            if (user == null)
                return default;

            return user;
        }

        public async Task<UserEntity?> FindByEmailAsync(string email, CancellationToken token = default)
        {
            var user = await _repo.FindByEmailAsync(email, token);
            if (user == null)
                return default;

            return user;
        }
        public async Task<UserEntity?> FindByEmailAndPasswordAsync(string email, string password, CancellationToken token = default)
        {
            var user = await _repo.FindByEmailAsync(email, token);
            if (user == null)
                return default;

            var pwd = _passwordHelper.GenerateHash(password, user.Salt);

            if (user.Password.SequenceEqual(pwd))
                return user;

            return default;
        }

        public async Task<UserEntity> CreateAsync(UserCreateModel user, long createdByUserId, CancellationToken token)
        {
            if (user.Role == RoleEnum.SuperAdmin)
                throw new InvalidOperationException("Cannot create another super admin user");


            var userExists = await _repo.FindByEmailAsync(user.Email, token);

            if (userExists != default)
                throw new PetRadarException("Can't create duplicated data");


            var salt = _passwordHelper.GenerateSalt();
            var hashPassword = _passwordHelper.GenerateHash(user.Password, salt);

            var userdb = new UserEntity(user.Email, hashPassword, salt, user.Name, user.LastName, user.PhoneNumber, 
                user.OrganizationName, user.OrganizationAddress, user.OrganizationPhone,user.Role, createdByUserId);

            await _repo.AddAsync(userdb);
            await _repo.SaveChangesAsync();
            return userdb;
        }

        public async Task<int> UpdateAsync(UserEntity userdb, UserUpdateModel user, long modifiedByUserId, CancellationToken token)
        {
            if (userdb == default)
                throw new ArgumentNullException(nameof(userdb));


            if (!string.IsNullOrEmpty(user.Email))
                userdb.Email = user.Email;

            if (!string.IsNullOrEmpty(user.Password))
            {
                userdb.Salt = _passwordHelper.GenerateSalt();

                userdb.Password = _passwordHelper.GenerateHash(user.Password, userdb.Salt);
            }

            if (!string.IsNullOrEmpty(user.Name))
                userdb.Name = user.Name;

            if (!string.IsNullOrEmpty(user.LastName))
                userdb.LastName = user.LastName;

            if (!string.IsNullOrEmpty(user.PhoneNumber))
                userdb.PhoneNumber = user.PhoneNumber;

            if (!string.IsNullOrEmpty(user.OrganizationName))
                userdb.OrganizationName = user.OrganizationName;

            if (!string.IsNullOrEmpty(user.OrganizationAddress))
                userdb.OrganizationAddress = user.OrganizationAddress;

            if (!string.IsNullOrEmpty(user.OrganizationPhone))
                userdb.OrganizationPhone = user.OrganizationPhone;

            if (user.Role.HasValue)
                userdb.Role = user.Role.Value;
            

            userdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(userdb);

            int result = await _repo.SaveChangesAsync();

            return result;
        }

        public async Task<int> VerifyEmailAsync(UserEntity userdb, long modifiedByUserId)
        {
            
            userdb.EmailVerified = true;
            userdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(userdb);

            int result = await _repo.SaveChangesAsync();
            return result;
        }
        public async Task<int> UpdateFcmTokenAsync(UserEntity userdb, string? fcmToken, long modifiedByUserId, CancellationToken token)
        {
            userdb.FcmToken = string.IsNullOrWhiteSpace(fcmToken) ? null : fcmToken;
            userdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(userdb);

            return await _repo.SaveChangesAsync();
        }

        public async Task<int> UpdateProfilePictureAsync(UserEntity userdb, IFormFile file, long modifiedByUserId, CancellationToken token)
        {

            if (userdb.ProfilePhotoURL != null)
            {
                _fileHelperService.DeleteImage(userdb.ProfilePhotoURL, _logger);

                userdb.ProfilePhotoURL = null;
            }

            string relativePath = await _fileHelperService.SaveImage(file);

            userdb.ProfilePhotoURL = relativePath;

            userdb.UpdatedByUser(modifiedByUserId);
            _repo.Update(userdb);

            int result = await _repo.SaveChangesAsync();
            return result;
        }

        public async Task<int> DeleteAsync(UserEntity user, long modifiedByUserId, CancellationToken token)
        {
            if (user == default)
                throw new ArgumentNullException(nameof(user));

            if (user.Role == RoleEnum.SuperAdmin)
                throw new InvalidOperationException("Cannot delete a super admin user");

            user.IsActive = false;

            // Delete profile picture if exists
            if (user.ProfilePhotoURL != null)
            {
                try
                {
                    _fileHelperService.DeleteImage(user.ProfilePhotoURL, _logger);

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while deleting image {message}", ex.Message);
                }

                user.ProfilePhotoURL = null;
            }

            user.DeletedByUser(modifiedByUserId);
            _repo.Update(user);

            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> RecoverPasswordAsync(UserEntity user, long modifiedByUserId, CancellationToken token)
        {

            string newPassword = _passwordHelper.GeneratePassword();

            user.Salt = _passwordHelper.GenerateSalt();
            user.Password = _passwordHelper.GenerateHash(newPassword, user.Salt);

            user.UpdatedByUser(modifiedByUserId);

            var response = await _emailHelperService.SendRecoverPasswordEmail(user, newPassword);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Recover password email sent successfully to {email}", user.Email);
                
                _repo.Update(user);

                await _repo.SaveChangesAsync();

                return true;
            }
            else
            {
                _logger.LogWarning("Failed to send recover password email to {email}: {statusCode} - {reasonPhrase}", user.Email, response.StatusCode, response.StatusDescription);
               
                return false;
            }
        }
    }
}
