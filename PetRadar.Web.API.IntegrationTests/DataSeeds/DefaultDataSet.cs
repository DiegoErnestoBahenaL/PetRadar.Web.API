using NetTopologySuite.Geometries;
using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain;
using PetRadar.Core.Helpers;
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

        public static readonly int DefaultPetId = 2000;
        public static readonly string DefaultPetName = "TestDog";
        public static readonly PetSpeciesEnum DefaultPetSpecies = PetSpeciesEnum.Dog;

        public static readonly int DefaultLostReportId = 3000;
        public static readonly int DefaultStrayReportId = 3001;

        public static readonly int DefaultRecipientUserId = 1001;
        public static readonly string DefaultRecipientUserEmail = "recipient@email.com";
        public static readonly string DefaultRecipientUserName = "Recipient";
        public static readonly string DefaultRecipientUserLastName = "Two";


        public UserEntity DefaultUserEntity { get; private set; }

        public UserEntity DefaultUser { get => DefaultUserEntity; }

        public UserPetEntity DefaultPetEntity { get; private set; }

        public ReportEntity DefaultLostReportEntity { get; private set; }

        public ReportEntity DefaultStrayReportEntity { get; private set; }

        public UserEntity DefaultRecipientUserEntity { get; private set; }

        public DefaultDataSet()
        {
            var passwordHelper = new PasswordHelper();
            var salt = passwordHelper.GenerateSalt();
            DefaultUserEntity = new UserEntity();
            DefaultPetEntity = new UserPetEntity();
            DefaultLostReportEntity = new ReportEntity();
            DefaultStrayReportEntity = new ReportEntity();
            DefaultRecipientUserEntity = new UserEntity();

            DefaultUserEntity = new UserEntity()
            {
                Id = DefaultUserId,
                Email = DefaultUserEmail,
                Salt = salt,
                Password = passwordHelper.GenerateHash(DefaultUserPassword, salt),
                Name = DefaultUserName,
                LastName = DefaultUserLastName,
                PhoneNumber = DefaultUserPhoneNumber,
                Role = DefaultUserRole,
                IsActive = true
            };

            var recipientSalt = passwordHelper.GenerateSalt();
            DefaultRecipientUserEntity = new UserEntity()
            {
                Id = DefaultRecipientUserId,
                Email = DefaultRecipientUserEmail,
                Salt = recipientSalt,
                Password = passwordHelper.GenerateHash(DefaultUserPassword, recipientSalt),
                Name = DefaultRecipientUserName,
                LastName = DefaultRecipientUserLastName,
                Role = RoleEnum.User,
                IsActive = true
            };


        }

        public void SeedData(PetRadarDbContext dbContext)
        {
            dbContext.Users.Add(DefaultUserEntity);
            dbContext.SaveChanges();

            dbContext.Users.Add(DefaultRecipientUserEntity);
            dbContext.SaveChanges();

            DefaultPetEntity = new UserPetEntity()
            {
                Id = DefaultPetId,
                UserId = DefaultUserId,
                Name = DefaultPetName,
                Species = DefaultPetSpecies,
                IsActive = true,
                CreatedBy = DefaultUserId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = DefaultUserId
            };

            dbContext.UserPets.Add(DefaultPetEntity);
            dbContext.SaveChanges();

            DefaultLostReportEntity = new ReportEntity()
            {
                Id = DefaultLostReportId,
                UserId = DefaultUserId,
                Species = PetSpeciesEnum.Dog,
                Breed = "Labrador",
                Color = "Golden",
                Sex = PetSexEnum.Male,
                Size = PetSizeEnum.Large,
                ApproximateAge = 3,
                Weight = 25.0m,
                Description = "Lost dog near the park",
                IsNeutered = true,
                ReportType = ReportTypeEnum.Lost,
                ReportStatus = ReportStatusEnum.Active,
                HasCollar = true,
                HasTag = false,
                Location = new Point(-99.1332, 19.4326) { SRID = 4326 },
                AddressText = "Test Address",
                SearchRadiusMeters = 5000,
                IsActive = true,
                CreatedBy = DefaultUserId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = DefaultUserId
            };

            DefaultStrayReportEntity = new ReportEntity()
            {
                Id = DefaultStrayReportId,
                UserId = DefaultUserId,
                Species = PetSpeciesEnum.Dog,
                Breed = "Labrador",
                Color = "Golden",
                Sex = PetSexEnum.Male,
                Size = PetSizeEnum.Large,
                ApproximateAge = 3,
                Weight = 25.0m,
                Description = "Stray dog found near the park",
                IsNeutered = true,
                ReportType = ReportTypeEnum.Stray,
                ReportStatus = ReportStatusEnum.Active,
                HasCollar = false,
                HasTag = false,
                Location = new Point(-99.1332, 19.4326) { SRID = 4326 },
                AddressText = "Test Address",
                SearchRadiusMeters = 5000,
                IsActive = true,
                CreatedBy = DefaultUserId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = DefaultUserId
            };

            dbContext.Reports.AddRange(DefaultLostReportEntity, DefaultStrayReportEntity);
            dbContext.SaveChanges();
        }
    }
}
