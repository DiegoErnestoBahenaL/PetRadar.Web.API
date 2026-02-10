using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Web.API.IntegrationTests.DataSeeds
{
    public class DefaultDataSet : IDefaultDataSet
    {

        public static readonly int DefaultUserId = 1000;
        public static readonly string DefaultUserEmail = "test@email.com";
        public static readonly string DefaultUserPassword = "password";
        public static readonly string DefaultUserName = "User";
        public static readonly string DefaultUserLastName = "One";
        public static readonly string DefaultUserPhoneNumber = "1234567890";
        public static readonly RoleEnum DefaultUserRole = RoleEnum.Admin;


        public UserEntity DefaultUserEntity { get; private set; }

        public UserEntity DefaultUser { get => DefaultUserEntity; }

        public DefaultDataSet()
        {
            DefaultUserEntity = new UserEntity();
        }

        public void SeedData(PetRadarDbContext dbContext)
        {

            var salt = UserDomain.GenerateSalt();
            DefaultUserEntity = new UserEntity()
            {
                Id = DefaultUserId,
                Email = DefaultUserEmail,
                Salt = salt,
                Password = UserDomain.GenerateHash(DefaultUserPassword, salt),
                Name = DefaultUserName,
                LastName = DefaultUserLastName,
                PhoneNumber = DefaultUserPhoneNumber,
                Role = DefaultUserRole,
                IsActive = true
            };
            

            dbContext.Users.Add(DefaultUserEntity);
            dbContext.SaveChanges();
        }
    }
}
