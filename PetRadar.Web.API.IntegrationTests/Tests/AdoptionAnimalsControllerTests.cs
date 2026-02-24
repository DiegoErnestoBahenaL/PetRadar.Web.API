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
    public class AdoptionAnimalsControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/adoptionanimals";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdAnimalId = 0;

        public AdoptionAnimalsControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Animals_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var animals = JsonConvert.DeserializeObject<List<AdoptionAnimalViewModel>>(stringContent);

            Assert.NotNull(animals);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAdoptionDog",
                species: PetSpeciesEnum.Dog,
                breed: "Labrador",
                color: "Golden",
                sex: PetSexEnum.Male,
                size: PetSizeEnum.Large,
                approximateAge: 3,
                weight: 25.0m,
                description: "A friendly dog looking for a home",
                isNeutered: true,
                personality: "Friendly",
                goodWithKids: true,
                goodWithDogs: true,
                goodWithCats: false,
                isVaccinated: true,
                needsSpecialCare: false,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdAnimal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(stringContent);

            Assert.NotNull(createdAnimal);
            Assert.True(createdAnimal.Id > 0);
            Assert.Equal(createModel.ShelterId, createdAnimal.ShelterId);
            Assert.Equal(createModel.Name, createdAnimal.Name);
            Assert.Equal(createModel.Species.ToString(), createdAnimal.Species);
            Assert.Equal(createModel.Breed, createdAnimal.Breed);
            Assert.Equal(createModel.Color, createdAnimal.Color);
            Assert.Equal(createModel.Sex.ToString(), createdAnimal.Sex);
            Assert.Equal(createModel.Size.ToString(), createdAnimal.Size);

            createdAnimalId = createdAnimal.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Animal_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdAnimalId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var animal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(stringContent);

            Assert.NotNull(animal);
            Assert.Equal(createdAnimalId, animal.Id);
            Assert.Equal("TestAdoptionDog", animal.Name);
            Assert.Equal("Dog", animal.Species);
            Assert.Equal("Labrador", animal.Breed);
        }

        [Fact, TestPriority(4)]
        public async Task GetByShelterId_Returns_Animals_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/shelter/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var animals = JsonConvert.DeserializeObject<List<AdoptionAnimalViewModel>>(stringContent);

            Assert.NotNull(animals);
            Assert.True(animals.Count > 0);
            Assert.Contains(animals, a => a.Id == createdAnimalId);
        }

        [Fact, TestPriority(5)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new AdoptionAnimalUpdateModel(
                name: "UpdatedAdoptionDog",
                species: PetSpeciesEnum.Cat,
                breed: "Persian",
                color: "White",
                sex: PetSexEnum.Female,
                size: PetSizeEnum.Medium,
                approximateAge: 5,
                weight: 4.5m,
                description: "An updated description",
                isNeutered: false,
                personality: "Calm",
                goodWithKids: false,
                goodWithDogs: false,
                goodWithCats: true,
                isVaccinated: false,
                needsSpecialCare: true,
                specialCareDetails: "Needs daily medication",
                status: AdoptionStatusEnum.Reserved,
                adoptionDate: null,
                adopterId: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAnimalId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task GetById_Returns_Updated_Animal_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdAnimalId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var animal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(stringContent);

            Assert.NotNull(animal);
            Assert.Equal(createdAnimalId, animal.Id);
            Assert.Equal("UpdatedAdoptionDog", animal.Name);
            Assert.Equal("Cat", animal.Species);
            Assert.Equal("Persian", animal.Breed);
            Assert.Equal("White", animal.Color);
            Assert.Equal("Calm", animal.Personality);
            Assert.Equal("Needs daily medication", animal.SpecialCareDetails);
        }

        [Fact, TestPriority(7)]
        public async Task Update_With_AdopterId_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new AdoptionAnimalUpdateModel(
                name: null,
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null,
                status: AdoptionStatusEnum.Adopted,
                adoptionDate: DateTimeOffset.UtcNow,
                adopterId: DefaultDataSet.DefaultUserId
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAnimalId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdAnimalId}");

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
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_NotFound_With_Invalid_ShelterId()
        {
            // Arrange
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: 999999,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

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
                ShelterId = DefaultDataSet.DefaultUserId
                // Missing Name and Species
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

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
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Name_Exceeding_MaxLength()
        {
            // Arrange
            var longName = new string('A', 101); // Exceeds 100 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: longName,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Breed_Exceeding_MaxLength()
        {
            // Arrange
            var longBreed = new string('A', 101); // Exceeds 100 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: longBreed,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Color_Exceeding_MaxLength()
        {
            // Arrange
            var longColor = new string('A', 101); // Exceeds 100 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: longColor,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Description_Exceeding_MaxLength()
        {
            // Arrange
            var longDescription = new string('A', 501); // Exceeds 500 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: longDescription,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Personality_Exceeding_MaxLength()
        {
            // Arrange
            var longPersonality = new string('A', 101); // Exceeds 100 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: longPersonality,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_SpecialCareDetails_Exceeding_MaxLength()
        {
            // Arrange
            var longSpecialCareDetails = new string('A', 501); // Exceeds 500 character limit
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: longSpecialCareDetails
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;
            var updateModel = new AdoptionAnimalUpdateModel(
                name: "UpdatedName",
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null,
                status: null,
                adoptionDate: null,
                adopterId: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{invalidId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_NotFound_With_Invalid_AdopterId()
        {
            // Arrange - First create an animal to update
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TempAnimal",
                species: PetSpeciesEnum.Cat,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdAnimal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(createContent);

            // Arrange update with invalid adopter id
            var updateModel = new AdoptionAnimalUpdateModel(
                name: null,
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null,
                status: AdoptionStatusEnum.Adopted,
                adoptionDate: DateTimeOffset.UtcNow,
                adopterId: 999999
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAnimal!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdAnimal.Id}");
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Name_Exceeding_MaxLength()
        {
            // Arrange - First create an animal to update
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "TempAnimal",
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdAnimal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(createContent);

            // Arrange update with invalid name
            var longName = new string('A', 101); // Exceeds 100 character limit
            var updateModel = new AdoptionAnimalUpdateModel(
                name: longName,
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                personality: null,
                goodWithKids: null,
                goodWithDogs: null,
                goodWithCats: null,
                isVaccinated: null,
                needsSpecialCare: null,
                specialCareDetails: null,
                status: null,
                adoptionDate: null,
                adopterId: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAnimal!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdAnimal.Id}");
        }

        [Fact]
        public async Task Delete_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        #endregion
    }
}
