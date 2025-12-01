using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.VisualBasic;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Repositories;
using PetRadar.Core.Domain.Models;
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
        
        public UserDomain(IUserRepository repo) {  _repo = repo; }

        public Task<List<UserEntity>> GetAllAsync(CancellationToken token)
        {
            return _repo.GetAllAsync(token);
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

            var pwd = GenerateHash(password, user.Salt);

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
                throw new Exception("Can't create duplicated data");


            var salt = GenerateSalt();
            var hashPassword = GenerateHash(user.Password, salt);

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
                userdb.Salt = GenerateSalt();

                userdb.Password = GenerateHash(user.Password, userdb.Salt);
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

        public async Task<int> DeleteAsync(UserEntity user, long modifiedByUserId, CancellationToken token)
        {
            if (user == default)
                throw new ArgumentNullException(nameof(user));

            if (user.Role == RoleEnum.SuperAdmin)
                throw new InvalidOperationException("Cannot delete a super admin user");

            user.IsActive = false;

            user.DeletedByUser(modifiedByUserId);
            _repo.Update(user);

            return await _repo.SaveChangesAsync();
        }


        public static byte[] GenerateHash(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Cannot be null or empty", nameof(password));

            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 10000, 256);
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128];
          
            RandomNumberGenerator.Fill(salt);

            return salt;
        }

    }
}
