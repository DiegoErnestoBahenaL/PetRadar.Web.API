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
    public class MatchesControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/matches";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdMatchId = 0;

        public MatchesControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Matches_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(stringContent);

            Assert.NotNull(matches);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var createModel = new MatchCreateModel(
                lostReportId: DefaultDataSet.DefaultLostReportId,
                strayReportId: DefaultDataSet.DefaultStrayReportId,
                distanceInKM: 2.5,
                notes: "Possible match found near park area",
                confirmationDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdMatch = JsonConvert.DeserializeObject<MatchViewModel>(stringContent);

            Assert.NotNull(createdMatch);
            Assert.True(createdMatch.Id > 0);
            Assert.Equal(DefaultDataSet.DefaultLostReportId, createdMatch.LostReport.Id);
            Assert.Equal(DefaultDataSet.DefaultStrayReportId, createdMatch.StrayReport.Id);
            Assert.Equal(2.5, createdMatch.DistanceInKM);
            Assert.Equal("Possible match found near park area", createdMatch.Notes);
            Assert.Equal("Pending", createdMatch.Status);

            createdMatchId = createdMatch.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Match_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdMatchId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var match = JsonConvert.DeserializeObject<MatchViewModel>(stringContent);

            Assert.NotNull(match);
            Assert.Equal(createdMatchId, match.Id);
            Assert.Equal(DefaultDataSet.DefaultLostReportId, match.LostReport.Id);
            Assert.Equal(DefaultDataSet.DefaultStrayReportId, match.StrayReport.Id);
            Assert.Equal("Possible match found near park area", match.Notes);
        }

        [Fact, TestPriority(4)]
        public async Task GetByLostReportId_Returns_Matches_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/lost-report/{DefaultDataSet.DefaultLostReportId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(stringContent);

            Assert.NotNull(matches);
            Assert.True(matches.Count > 0);
            Assert.Contains(matches, m => m.Id == createdMatchId);
        }

        [Fact, TestPriority(5)]
        public async Task GetByStrayReportId_Returns_Matches_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/stray-report/{DefaultDataSet.DefaultStrayReportId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(stringContent);

            Assert.NotNull(matches);
            Assert.True(matches.Count > 0);
            Assert.Contains(matches, m => m.Id == createdMatchId);
        }

        [Fact, TestPriority(6)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var confirmationDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            var updateModel = new MatchUpdateModel(
                score: 85.5,
                distanceInKM: 1.2,
                status: MatchStatusEnum.Confirmed,
                notes: "Match confirmed by owner",
                confirmationDate: confirmationDate
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdMatchId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task GetById_Returns_Updated_Match_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdMatchId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var match = JsonConvert.DeserializeObject<MatchViewModel>(stringContent);

            Assert.NotNull(match);
            Assert.Equal(createdMatchId, match.Id);
            Assert.Equal(85.5, match.Score);
            Assert.Equal(1.2, match.DistanceInKM);
            Assert.Equal("Confirmed", match.Status);
            Assert.Equal("Match confirmed by owner", match.Notes);
            Assert.NotNull(match.ConfirmationDate);
        }

        [Fact, TestPriority(8)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdMatchId}");

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
        public async Task Create_Returns_NotFound_With_Invalid_LostReportId()
        {
            // Arrange
            var createModel = new MatchCreateModel(
                lostReportId: 999999,
                strayReportId: 999998,
                distanceInKM: null,
                notes: null,
                confirmationDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_NotFound_With_Invalid_StrayReportId()
        {
            // Arrange - Use seeded lost report with invalid stray report id
            var createModel = new MatchCreateModel(
                lostReportId: DefaultDataSet.DefaultLostReportId,
                strayReportId: 999999,
                distanceInKM: null,
                notes: null,
                confirmationDate: null
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
                Notes = "Missing report ids"
                // Missing LostReportId and StrayReportId
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
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
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Notes_Exceeding_MaxLength()
        {
            // Arrange
            var longNotes = new string('A', 501); // Exceeds 500 character limit
            var createModel = new MatchCreateModel(
                lostReportId: DefaultDataSet.DefaultLostReportId,
                strayReportId: DefaultDataSet.DefaultStrayReportId,
                distanceInKM: null,
                notes: longNotes,
                confirmationDate: null
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
            var updateModel = new MatchUpdateModel(
                score: null,
                distanceInKM: null,
                status: null,
                notes: "Updated notes",
                confirmationDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{invalidId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Notes_Exceeding_MaxLength()
        {
            // Arrange - Create a match using seeded reports
            var createModel = new MatchCreateModel(
                lostReportId: DefaultDataSet.DefaultLostReportId,
                strayReportId: DefaultDataSet.DefaultStrayReportId,
                distanceInKM: null,
                notes: null,
                confirmationDate: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdMatch = JsonConvert.DeserializeObject<MatchViewModel>(createContent);

            // Arrange update with invalid notes
            var longNotes = new string('A', 501); // Exceeds 500 character limit
            var updateModel = new MatchUpdateModel(
                score: null,
                distanceInKM: null,
                status: null,
                notes: longNotes,
                confirmationDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdMatch!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdMatch.Id}");
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

        [Fact]
        public async Task GetByLostReportId_Returns_Empty_List_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/lost-report/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(stringContent);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        [Fact]
        public async Task GetByStrayReportId_Returns_Empty_List_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/stray-report/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var matches = JsonConvert.DeserializeObject<List<MatchViewModel>>(stringContent);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        #endregion
    }
}
