using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.IntegrationTests.DataSeeds;
using PetRadar.Web.API.IntegrationTests.Helpers;
using PetRadar.Web.API.ViewModels;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Xunit;

namespace PetRadar.Web.API.IntegrationTests.Tests
{
    [TestCaseOrderer(PriorityOrderer.PriorityOrdererName, PriorityOrderer.PriorityOrdererAssemblyName)]
    public class AdoptionAnimalsImageControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;
        
        static long createdAnimalId = 0;
        static string? uploadedAdditionalPhotoName = null;

        private string GetAssetPath(string fileName)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string targetToReplace = Path.Combine("bin", "Debug", "net8.0");
            basePath = basePath.Replace(targetToReplace, string.Empty);
            return Path.Combine(basePath, "Assets", fileName);
        }

        public AdoptionAnimalsImageControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region Happy Path (Priority Order)

        [Fact, TestPriority(1)]
        public async Task Setup_Create_Animal_For_Images_Successfully()
        {
            var createModel = new AdoptionAnimalCreateModel(
                shelterId: DefaultDataSet.DefaultUserId,
                name: "ImageTestAnimal",
                species: PetSpeciesEnum.Dog,
                breed: "Labrador",
                color: "Golden",
                sex: PetSexEnum.Male,
                size: PetSizeEnum.Large,
                approximateAge: 2,
                weight: 25.0m,
                description: "A friendly dog for rescue image testing",
                isNeutered: true,
                personality: "Playful",
                goodWithKids: true,
                goodWithDogs: true,
                goodWithCats: false,
                isVaccinated: true,
                needsSpecialCare: false,
                specialCareDetails: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);
            var result = await _client.PostAsync("/api/adoptionanimals", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));
            var stringContent = await result.Content.ReadAsStringAsync();
            var createdAnimal = JsonConvert.DeserializeObject<AdoptionAnimalViewModel>(stringContent);

            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);
            Assert.NotNull(createdAnimal);
            createdAnimalId = createdAnimal.Id;
        }

        [Fact, TestPriority(2)]
        public async Task UploadMainPicture_Returns_NoContent_Successfully()
        {
            string filePath = GetAssetPath("Dog.jpg");
            using var fileStream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "file", "Dog.jpg");

            var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/mainpicture", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(3)]
        public async Task GetMainPicture_Returns_Image_Successfully()
        {
            var result = await _client.GetAsync($"/api/adoptionanimals/{createdAnimalId}/mainpicture");

            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.Contains("image/", result.Content.Headers.ContentType?.MediaType);
        }

        [Fact, TestPriority(4)]
        public async Task UploadAdditionalPhotos_Returns_NoContent_Successfully()
        {
            var filePath1 = GetAssetPath("Dog.jpg");
            using var content = new MultipartFormDataContent();
            
            var streamContent1 = new StreamContent(File.OpenRead(filePath1));
            streamContent1.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent1, "files", "Dog.jpg");

            var streamContent2 = new StreamContent(File.OpenRead(filePath1));
            streamContent2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent2, "files", "Dog.jpg");

            var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task GetAdditionalPhotosNames_Returns_Names_Successfully()
        {
            var result = await _client.GetAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos");
            var contentString = await result.Content.ReadAsStringAsync();
            var photos = JsonConvert.DeserializeObject<List<string>>(contentString);

            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.NotNull(photos);
            Assert.True(photos.Count >= 2);
            
            uploadedAdditionalPhotoName = photos.First();
        }

        [Fact, TestPriority(6)]
        public async Task GetSpecificAdditionalPhoto_Returns_Image_Successfully()
        {
            var result = await _client.GetAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos/{uploadedAdditionalPhotoName}");

            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.Contains("image/", result.Content.Headers.ContentType?.MediaType);
        }

        [Fact, TestPriority(7)]
        public async Task DeleteAdditionalPhoto_Returns_NoContent_Successfully()
        {
            var result = await _client.DeleteAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos/{uploadedAdditionalPhotoName}");

            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task UploadMainPicture_Returns_BadRequest_With_Specie_Validation_Failure()
        {
            var filePath = GetAssetPath("Cat.jpg");
            if (File.Exists(filePath))
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(File.OpenRead(filePath));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "file", "Cat.jpg");

                var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/mainpicture", content);

                Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        [Fact, TestPriority(9)]
        public async Task UploadMainPicture_With_Invalid_FileFormat_Returns_BadRequest()
        {
            var filePath = GetAssetPath("InvalidImage.jpg");
            if (File.Exists(filePath))
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(File.OpenRead(filePath));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "file", "InvalidImage.jpg");

                var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/mainpicture", content);

                Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        [Fact, TestPriority(10)]
        public async Task UploadAdditionalPhotos_Returns_BadRequest_With_Specie_Validation_Failure()
        {
            var filePath = GetAssetPath("Cat.jpg");
            if (File.Exists(filePath))
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(File.OpenRead(filePath));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "files", "Cat.jpg");

                var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos", content);

                Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        [Fact, TestPriority(11)]
        public async Task UploadAdditionalPhotos_With_Invalid_FileFormat_Returns_BadRequest()
        {
            var filePath = GetAssetPath("InvalidImage.jpg");
            if (File.Exists(filePath))
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(File.OpenRead(filePath));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "files", "InvalidImage.jpg");

                var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos", content);

                Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region Negative Scenarios (No Priority)

        [Fact]
        public async Task UploadMainPicture_Returns_NotFound_With_Invalid_AnimalId()
        {
            var filePath = GetAssetPath("Dog.jpg");
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.OpenRead(filePath));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "file", "Dog.jpg");
            var result = await _client.PutAsync("/api/adoptionanimals/999999/mainpicture", content);

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetMainPicture_Returns_NotFound_With_Invalid_AnimalId()
        {
            var result = await _client.GetAsync("/api/adoptionanimals/999999/mainpicture");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task UploadAdditionalPhotos_Returns_BadRequest_With_No_Files()
        {
            using var content = new MultipartFormDataContent();

            var result = await _client.PutAsync($"/api/adoptionanimals/{createdAnimalId}/additionalphotos", content);

            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task UploadAdditionalPhotos_Returns_NotFound_With_Invalid_AnimalId()
        {
            var filePath = GetAssetPath("Dog.jpg");
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.OpenRead(filePath));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "files", "Dog.jpg");
            
            var result = await _client.PutAsync("/api/adoptionanimals/999999/additionalphotos", content);

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetAdditionalPhotosNames_Returns_NotFound_With_Invalid_AnimalId()
        {
            var result = await _client.GetAsync("/api/adoptionanimals/999999/additionalphotos");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetSpecificAdditionalPhoto_Returns_NotFound_With_Invalid_AnimalId()
        {
            var result = await _client.GetAsync("/api/adoptionanimals/999999/additionalphotos/mockedname.jpg");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task DeleteAdditionalPhoto_Returns_NotFound_With_Invalid_AnimalId()
        {
            var result = await _client.DeleteAsync("/api/adoptionanimals/999999/additionalphotos/mockedname.jpg");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        #endregion
    }
}
