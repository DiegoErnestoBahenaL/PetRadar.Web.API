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
    public class ReportsControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/reports";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdReportId = 0;

        public ReportsControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Reports_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var reports = JsonConvert.DeserializeObject<List<ReportViewModel>>(stringContent);

            Assert.NotNull(reports);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var incidentDate = new DateTimeOffset(DateTime.UtcNow.AddHours(-2), TimeSpan.Zero);

            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: DefaultDataSet.DefaultPetId,
                species: PetSpeciesEnum.Dog,
                breed: "Labrador",
                color: "Golden",
                sex: PetSexEnum.Male,
                size: PetSizeEnum.Large,
                approximateAge: 3,
                weight: 25.0m,
                description: "Lost dog near the park",
                isNeutered: true,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: true,
                hasTag: true,
                incidentDate: incidentDate,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: "123 Main Street",
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: true,
                rewardAmount: 500.00m
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdReport = JsonConvert.DeserializeObject<ReportViewModel>(stringContent);

            Assert.NotNull(createdReport);
            Assert.True(createdReport.Id > 0);
            Assert.Equal(createModel.UserId, createdReport.UserId);
            Assert.Equal(createModel.UserPetId, createdReport.UserPetId);
            Assert.Equal(createModel.Species.ToString(), createdReport.Species);
            Assert.Equal(createModel.Breed, createdReport.Breed);
            Assert.Equal(createModel.Color, createdReport.Color);
            Assert.Equal(createModel.ReportType.ToString(), createdReport.ReportType);
            Assert.Equal(createModel.ReportStatus.ToString(), createdReport.ReportStatus);

            createdReportId = createdReport.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Report_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdReportId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var report = JsonConvert.DeserializeObject<ReportViewModel>(stringContent);

            Assert.NotNull(report);
            Assert.Equal(createdReportId, report.Id);
            Assert.Equal("Dog", report.Species);
            Assert.Equal("Labrador", report.Breed);
            Assert.Equal("Lost", report.ReportType);
            Assert.Equal("Lost dog near the park", report.Description);
        }

        [Fact, TestPriority(4)]
        public async Task GetByUserId_Returns_Reports_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/user/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var reports = JsonConvert.DeserializeObject<List<ReportViewModel>>(stringContent);

            Assert.NotNull(reports);
            Assert.True(reports.Count > 0);
            Assert.Contains(reports, r => r.Id == createdReportId);
        }

        [Fact, TestPriority(5)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new ReportUpdateModel(
                species: PetSpeciesEnum.Cat,
                breed: "Persian",
                color: "White",
                sex: PetSexEnum.Female,
                size: PetSizeEnum.Medium,
                approximateAge: 2,
                weight: 4.5m,
                description: "Updated report description",
                isNeutered: false,
                reportType: ReportTypeEnum.Found,
                reportStatus: ReportStatusEnum.Resolved,
                hasCollar: false,
                hasTag: false,
                incidentDate: null,
                latitude: 20.0,
                longitude: -100.0,
                addressText: "456 Updated Street",
                searchRadiusMeters: 10000,
                useAlternateContact: true,
                contactName: "John Doe",
                contactPhone: "5551234567",
                contactEmail: "john@test.com",
                offersReward: false,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdReportId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task GetById_Returns_Updated_Report_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdReportId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var report = JsonConvert.DeserializeObject<ReportViewModel>(stringContent);

            Assert.NotNull(report);
            Assert.Equal(createdReportId, report.Id);
            Assert.Equal("Cat", report.Species);
            Assert.Equal("Persian", report.Breed);
            Assert.Equal("White", report.Color);
            Assert.Equal("Found", report.ReportType);
            Assert.Equal("Resolved", report.ReportStatus);
            Assert.Equal("Updated report description", report.Description);
            Assert.Equal("456 Updated Street", report.AddressText);
            Assert.Equal("John Doe", report.ContactName);
            Assert.Equal("5551234567", report.ContactPhone);
            Assert.Equal("john@test.com", report.ContactEmail);
        }

        [Fact, TestPriority(7)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdReportId}");

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
        public async Task Create_Returns_NotFound_With_Invalid_UserId()
        {
            // Arrange
            var createModel = new ReportCreateModel(
                userId: 999999,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
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
                UserId = DefaultDataSet.DefaultUserId
                // Missing Species, ReportType, Latitude, Longitude
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
        public async Task Create_Returns_BadRequest_With_Breed_Exceeding_MaxLength()
        {
            // Arrange
            var longBreed = new string('A', 101); // Exceeds 100 character limit
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: longBreed,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
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
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: longColor,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
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
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: longDescription,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_AddressText_Exceeding_MaxLength()
        {
            // Arrange
            var longAddress = new string('A', 501); // Exceeds 500 character limit
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: longAddress,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_ContactName_Exceeding_MaxLength()
        {
            // Arrange
            var longContactName = new string('A', 101); // Exceeds 100 character limit
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: true,
                contactName: longContactName,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_ContactPhone_Exceeding_MaxLength()
        {
            // Arrange
            var longContactPhone = new string('1', 21); // Exceeds 20 character limit
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: true,
                contactName: null,
                contactPhone: longContactPhone,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_ContactEmail_Exceeding_MaxLength()
        {
            // Arrange
            var longContactEmail = new string('A', 256); // Exceeds 255 character limit
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Dog,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Lost,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: true,
                contactName: null,
                contactPhone: null,
                contactEmail: longContactEmail,
                offersReward: false,
                rewardAmount: null
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
            var updateModel = new ReportUpdateModel(
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: "Updated description",
                isNeutered: null,
                reportType: null,
                reportStatus: null,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: null,
                longitude: null,
                addressText: null,
                searchRadiusMeters: null,
                useAlternateContact: null,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: null,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{invalidId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Description_Exceeding_MaxLength()
        {
            // Arrange - First create a report to update
            var createModel = new ReportCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                userPetId: null,
                species: PetSpeciesEnum.Cat,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: null,
                isNeutered: null,
                reportType: ReportTypeEnum.Stray,
                reportStatus: ReportStatusEnum.Active,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: 19.4326,
                longitude: -99.1332,
                addressText: null,
                searchRadiusMeters: 5000,
                useAlternateContact: false,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: false,
                rewardAmount: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdReport = JsonConvert.DeserializeObject<ReportViewModel>(createContent);

            // Arrange update with invalid description
            var longDescription = new string('A', 501); // Exceeds 500 character limit
            var updateModel = new ReportUpdateModel(
                species: null,
                breed: null,
                color: null,
                sex: null,
                size: null,
                approximateAge: null,
                weight: null,
                description: longDescription,
                isNeutered: null,
                reportType: null,
                reportStatus: null,
                hasCollar: null,
                hasTag: null,
                incidentDate: null,
                latitude: null,
                longitude: null,
                addressText: null,
                searchRadiusMeters: null,
                useAlternateContact: null,
                contactName: null,
                contactPhone: null,
                contactEmail: null,
                offersReward: null,
                rewardAmount: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdReport!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdReport.Id}");
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
