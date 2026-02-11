using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.IntegrationTests.DataSeeds;
using PetRadar.Web.API.IntegrationTests.Helpers;
using PetRadar.Web.API.ViewModels;
using System.Net.Mime;
using System.Text;
using Xunit;

namespace PetRadar.Web.API.IntegrationTests.Tests
{
    [TestCaseOrderer(PriorityOrderer.PriorityOrdererName, PriorityOrderer.PriorityOrdererAssemblyName)]
    public class UsersControllerTests : IClassFixture<TestingWebAppFactory>
    {
        readonly TestingWebAppFactory _factory;

        static long createdUserId = 0;

        public UsersControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Users_Successfully()
        {
            // Arrange & Act
            var client = _factory.CreateClient();
            var result = await client.GetAsync("/api/users");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserViewModel>>(stringContent);

            Assert.NotNull(users);
            Assert.True(users.Count > 0);
        }

        [Fact, TestPriority(2)]
        public async Task GetById_Returns_User_Successfully()
        {
            // Arrange
            var client = _factory.CreateClient();
            var userId = DefaultDataSet.DefaultUserId;

            // Act
            var result = await client.GetAsync($"/api/users/{userId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(stringContent);

            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
            Assert.Equal(DefaultDataSet.DefaultUserEmail, user.Email);
            Assert.Equal(DefaultDataSet.DefaultUserName, user.Name);
        }

        [Fact, TestPriority(3)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createModel = new UserCreateModel(
                email: "newuser@test.com",
                password: "newpassword123",
                name: "NewUser",
                lastName: "TestLastName",
                phoneNumber: "9876543210",
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: RoleEnum.User
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdUser = JsonConvert.DeserializeObject<UserViewModel>(stringContent);

            Assert.NotNull(createdUser);
            Assert.True(createdUser.Id > 0);
            Assert.Equal(createModel.Email, createdUser.Email);
            Assert.Equal(createModel.Name, createdUser.Name);

            createdUserId = createdUser.Id;
        }

        [Fact, TestPriority(4)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var client = _factory.CreateClient();

            var updateModel = new UserUpdateModel(
                email: null,
                password: null,
                name: "UpdatedName",
                lastName: "UpdatedLastName",
                phoneNumber: "1112223333",
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await client.PutAsync($"/api/users/{createdUserId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task GetById_Returns_Updated_User_Successfully()
        {
            // Arrange & Act
            var client = _factory.CreateClient();

            var result = await client.GetAsync($"/api/users/{createdUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserViewModel>(stringContent);

            Assert.NotNull(user);
            Assert.Equal(createdUserId, user.Id);
            Assert.Equal("UpdatedName", user.Name);
            Assert.Equal("UpdatedLastName", user.LastName);
            Assert.Equal("1112223333", user.PhoneNumber);
        }

        [Fact, TestPriority(6)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var client = _factory.CreateClient();
            var result = await client.DeleteAsync($"/api/users/{createdUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        #endregion

        #region Negative Scenarios (No Priority)

        [Fact]
        public async Task GetById_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidUserId = 999999;

            // Act
            var result = await client.GetAsync($"/api/users/{invalidUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Invalid_Email()
        {
            // Arrange
            var client = _factory.CreateClient();
            var createModel = new UserCreateModel
            {
                Email = "invalidemail",
                Password = "password123",
                Name = "TestUser",
                Role = RoleEnum.User
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Missing_Required_Fields()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createModel = new
            {
                Email = "test@email.com"
                // Missing Password and Name
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Empty_Body()
        {
            // Arrange
            var client = _factory.CreateClient();
            var jsonModel = JsonConvert.SerializeObject(new { });

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidUserId = 999999;
            var updateModel = new UserUpdateModel(
                email: null,
                password: null,
                name: "UpdatedName",
                lastName: null,
                phoneNumber: null,
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await client.PutAsync($"/api/users/{invalidUserId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Invalid_Email_Format()
        {
            // Arrange
            var client = _factory.CreateClient();

            var userId = DefaultDataSet.DefaultUserId;
            var updateModel = new UserUpdateModel(
                email: "notavalidemail",
                password: null,
                name: null,
                lastName: null,
                phoneNumber: null,
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await client.PutAsync($"/api/users/{userId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidUserId = 999999;

            // Act
            var result = await client.DeleteAsync($"/api/users/{invalidUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Duplicate_Email()
        {
            // Arrange - Try to create user with existing email

            var client = _factory.CreateClient();

            var createModel = new UserCreateModel(
                email: DefaultDataSet.DefaultUserEmail,
                password: "password123",
                name: "DuplicateUser",
                lastName: "Test",
                phoneNumber: "1234567890",
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: RoleEnum.User
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status409Conflict, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_PhoneNumber_Exceeding_MaxLength()
        {
            // Arrange
            var client = _factory.CreateClient();

            var userId = DefaultDataSet.DefaultUserId;
            var updateModel = new UserUpdateModel(
                email: null,
                password: null,
                name: null,
                lastName: null,
                phoneNumber: "123456789012345678901", // Exceeds 20 character limit
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await client.PutAsync($"/api/users/{userId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Name_Exceeding_MaxLength()
        {
            // Arrange
            var client = _factory.CreateClient();

            var longName = new string('A', 256); // Exceeds 255 character limit
            var createModel = new UserCreateModel(
                email: "longname@test.com",
                password: "password123",
                name: longName,
                lastName: null,
                phoneNumber: null,
                organizationName: null,
                organizationAddress: null,
                organizationPhone: null,
                role: RoleEnum.User
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await client.PostAsync("/api/users", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        #endregion
    }
}
