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
    public class UserPetsControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdPetId = 0;

        public UserPetsControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Pets_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync("/api/userpets");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var pets = JsonConvert.DeserializeObject<List<UserPetViewModel>>(stringContent);

            Assert.NotNull(pets);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {

            var birthDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);   
            


            // Arrange
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: "Labrador",
                color: "Golden",
                sex: PetSexEnum.Male,
                size: PetSizeEnum.Large,
                birthDate: birthDate,
                approximateAge: 2,
                weight: 30.5m,
                description: "A friendly dog",
                isNeutered: true,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdPet = JsonConvert.DeserializeObject<UserPetViewModel>(stringContent);

            Assert.NotNull(createdPet);
            Assert.True(createdPet.Id > 0);
            Assert.Equal(createModel.Name, createdPet.Name);
            Assert.Equal(createModel.Species.ToString(), createdPet.Species);
            Assert.Equal(createModel.UserId, createdPet.UserId);

            createdPetId = createdPet.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Pet_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"/api/userpets/{createdPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var pet = JsonConvert.DeserializeObject<UserPetViewModel>(stringContent);

            Assert.NotNull(pet);
            Assert.Equal(createdPetId, pet.Id);
            Assert.Equal("TestPet", pet.Name);
        }

        [Fact, TestPriority(4)]
        public async Task GetByUserId_Returns_Pets_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"/api/userpets/user/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var pets = JsonConvert.DeserializeObject<List<UserPetViewModel>>(stringContent);

            Assert.NotNull(pets);
            Assert.True(pets.Count > 0);
            Assert.Contains(pets, p => p.Id == createdPetId);
        }

        [Fact, TestPriority(5)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new UserPetUpdateModel(
                name: "UpdatedPetName",
                species: PetSpeciesEnum.Cat,
                breed: "Persian",
                color: "White",
                sex: PetSexEnum.Female,
                size: PetSizeEnum.Medium,
                birthDate: null,
                approximateAge: 3,
                weight: 5.5m,
                description: "An updated pet description",
                isNeutered: false,
                allergies: "None",
                medicalNotes: "Healthy"
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"/api/userpets/{createdPetId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task GetById_Returns_Updated_Pet_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"/api/userpets/{createdPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var pet = JsonConvert.DeserializeObject<UserPetViewModel>(stringContent);

            Assert.NotNull(pet);
            Assert.Equal(createdPetId, pet.Id);
            Assert.Equal("UpdatedPetName", pet.Name);
            Assert.Equal("Cat", pet.Species);
            Assert.Equal("Persian", pet.Breed);
        }

        [Fact, TestPriority(7)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"/api/userpets/{createdPetId}");

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
            var invalidPetId = 999999;

            // Act
            var result = await _client.GetAsync($"/api/userpets/{invalidPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_NotFound_With_Invalid_UserId()
        {
            // Arrange
            var createModel = new UserPetCreateModel(
                userId: 999999,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Missing_Required_Fields()
        {
            // Arrange
            var createModel = new
            {
                UserId = DefaultDataSet.DefaultUserId
                // Missing Name and Species
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Empty_Body()
        {
            // Arrange
            var jsonModel = JsonConvert.SerializeObject(new { });

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Name_Exceeding_MaxLength()
        {
            // Arrange
            var longName = new string('A', 101); // Exceeds 100 character limit
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: longName,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Description_Exceeding_MaxLength()
        {
            // Arrange
            var longDescription = new string('A', 501); // Exceeds 500 character limit
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: longDescription,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidPetId = 999999;
            var updateModel = new UserPetUpdateModel(
                name: "UpdatedName",
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"/api/userpets/{invalidPetId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Name_Exceeding_MaxLength()
        {
            // Arrange - First create a pet to update
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TempPet",
                species: PetSpeciesEnum.Cat,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync("/api/userpets", new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdPet = JsonConvert.DeserializeObject<UserPetViewModel>(createContent);

            // Arrange update with invalid name
            var longName = new string('A', 101); // Exceeds 100 character limit
            var updateModel = new UserPetUpdateModel(
                name: longName,
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"/api/userpets/{createdPet!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"/api/userpets/{createdPet.Id}");
        }

        [Fact]
        public async Task Delete_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidPetId = 999999;

            // Act
            var result = await _client.DeleteAsync($"/api/userpets/{invalidPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Breed_Exceeding_MaxLength()
        {
            // Arrange
            var longBreed = new string('A', 101); // Exceeds 100 character limit
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: longBreed,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Allergies_Exceeding_MaxLength()
        {
            // Arrange
            var longAllergies = new string('A', 256); // Exceeds 255 character limit
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: longAllergies,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_MedicalNotes_Exceeding_MaxLength()
        {
            // Arrange
            var longMedicalNotes = new string('A', 501); // Exceeds 500 character limit
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "TestPet",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                birthDate: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                allergies: null,
                medicalNotes: longMedicalNotes
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        #endregion
    }
}
