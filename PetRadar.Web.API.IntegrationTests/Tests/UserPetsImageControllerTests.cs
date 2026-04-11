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
    public class UserPetsImageControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;
        
        static long createdPetId = 0;
        static string? uploadedAdditionalPhotoName = null;

        private string GetAssetPath(string fileName)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string targetToReplace = Path.Combine("bin", "Debug", "net8.0");
            basePath = basePath.Replace(targetToReplace, string.Empty);
            return Path.Combine(basePath, "Assets", fileName);
        }

        public UserPetsImageControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region Happy Path (Priority Order)

        [Fact, TestPriority(1)]
        public async Task Setup_Create_Pet_For_Images_Successfully()
        {
            var createModel = new UserPetCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                name: "ImageTestPet",
                species: PetSpeciesEnum.Dog,
                breed: "Golden Retriever",
                color: "Golden",
                sex: PetSexEnum.Male,
                size: PetSizeEnum.Large,
                birthDate: new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                approximateAge: 2,
                weight: 30.5m,
                description: "A friendly dog for image testing",
                isNeutered: true,
                allergies: null,
                medicalNotes: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);
            var result = await _client.PostAsync("/api/userpets", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));
            var stringContent = await result.Content.ReadAsStringAsync();
            var createdPet = JsonConvert.DeserializeObject<UserPetViewModel>(stringContent);

            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);
            Assert.NotNull(createdPet);
            createdPetId = createdPet.Id;
        }

        [Fact, TestPriority(2)]
        public async Task UploadMainPicture_Returns_NoContent_Successfully()
        {
            // Arrange
            string filePath = GetAssetPath("Dog.jpg");
            using var fileStream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "file", "Dog.jpg");

            // Act
            var result = await _client.PutAsync($"/api/userpets/{createdPetId}/mainpicture", content);

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(3)]
        public async Task GetMainPicture_Returns_Image_Successfully()
        {
            // Act
            var result = await _client.GetAsync($"/api/userpets/{createdPetId}/mainpicture");

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.Contains("image/", result.Content.Headers.ContentType?.MediaType);
        }

        [Fact, TestPriority(4)]
        public async Task UploadAdditionalPhotos_Returns_NoContent_Successfully()
        {
            // Arrange
            var filePath1 = GetAssetPath("Dog.jpg");
            
            using var content = new MultipartFormDataContent();
            
            var streamContent1 = new StreamContent(File.OpenRead(filePath1));
            streamContent1.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent1, "files", "Dog.jpg");

            var streamContent2 = new StreamContent(File.OpenRead(filePath1));
            streamContent2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent2, "files", "Dog.jpg");
            // Act
            var result = await _client.PutAsync($"/api/userpets/{createdPetId}/additionalphotos", content);

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task GetAdditionalPhotosNames_Returns_Names_Successfully()
        {
            // Act
            var result = await _client.GetAsync($"/api/userpets/{createdPetId}/additionalphotos");
            var contentString = await result.Content.ReadAsStringAsync();
            var photos = JsonConvert.DeserializeObject<List<string>>(contentString);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.NotNull(photos);
            Assert.True(photos.Count >= 2);
            
            // Save one name for the next tests
            uploadedAdditionalPhotoName = photos.First();
        }

        [Fact, TestPriority(6)]
        public async Task GetSpecificAdditionalPhoto_Returns_Image_Successfully()
        {
            // Act
            var result = await _client.GetAsync($"/api/userpets/{createdPetId}/additionalphotos/{uploadedAdditionalPhotoName}");

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);
            Assert.Contains("image/", result.Content.Headers.ContentType?.MediaType);
        }

        [Fact, TestPriority(7)]
        public async Task DeleteAdditionalPhoto_Returns_NoContent_Successfully()
        {
            // Act
            var result = await _client.DeleteAsync($"/api/userpets/{createdPetId}/additionalphotos/{uploadedAdditionalPhotoName}");

            // Assert
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

                var result = await _client.PutAsync($"/api/userpets/{createdPetId}/mainpicture", content);

                // Asumiendo que atrapa un BadHttpRequestException en el controlador como 400
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

                var result = await _client.PutAsync($"/api/userpets/{createdPetId}/mainpicture", content);

                // Dependiendo de tu implementación exacta podría ser 400 o fallar la validación.
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

                var result = await _client.PutAsync($"/api/userpets/{createdPetId}/additionalphotos", content);

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

                var result = await _client.PutAsync($"/api/userpets/{createdPetId}/additionalphotos", content);

                Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region Negative Scenarios (No Priority)

        [Fact]
        public async Task UploadMainPicture_Returns_NotFound_With_Invalid_PetId()
        {
            var filePath = GetAssetPath("Dog.jpg");
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.OpenRead(filePath));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "file", "Dog.jpg");
            var result = await _client.PutAsync("/api/userpets/999999/mainpicture", content);

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetMainPicture_Returns_NotFound_With_Invalid_PetId()
        {
            var result = await _client.GetAsync("/api/userpets/999999/mainpicture");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task UploadAdditionalPhotos_Returns_BadRequest_With_No_Files()
        {
            using var content = new MultipartFormDataContent(); // Empty content

            var result = await _client.PutAsync($"/api/userpets/{createdPetId}/additionalphotos", content);

            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task UploadAdditionalPhotos_Returns_NotFound_With_Invalid_PetId()
        {
            var filePath = GetAssetPath("Dog.jpg");
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.OpenRead(filePath));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(streamContent, "files", "Dog.jpg");
            var result = await _client.PutAsync("/api/userpets/999999/additionalphotos", content);

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetAdditionalPhotosNames_Returns_NotFound_With_Invalid_PetId()
        {
            var result = await _client.GetAsync("/api/userpets/999999/additionalphotos");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetSpecificAdditionalPhoto_Returns_NotFound_With_Invalid_PetId()
        {
            var result = await _client.GetAsync("/api/userpets/999999/additionalphotos/mockedname.jpg");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task DeleteAdditionalPhoto_Returns_NotFound_With_Invalid_PetId()
        {
            var result = await _client.DeleteAsync("/api/userpets/999999/additionalphotos/mockedname.jpg");

            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }
        #endregion
    }
}