using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
    public class MessagesControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/messages";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdMessageId = 0;

        public MessagesControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Messages_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<MessageViewModel>>(stringContent);

            Assert.NotNull(messages);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var createModel = new MessageCreateModel(
                senderId: DefaultDataSet.DefaultUserId,
                recipientId: DefaultDataSet.DefaultRecipientUserId,
                content: "Hello, I think I found your pet!",
                reportId: DefaultDataSet.DefaultLostReportId,
                matchId: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdMessage = JsonConvert.DeserializeObject<MessageViewModel>(stringContent);

            Assert.NotNull(createdMessage);
            Assert.True(createdMessage.Id > 0);
            Assert.Equal(DefaultDataSet.DefaultUserId, createdMessage.SenderId);
            Assert.Equal(DefaultDataSet.DefaultRecipientUserId, createdMessage.RecipientId);
            Assert.Equal("Hello, I think I found your pet!", createdMessage.Content);
            Assert.Equal(DefaultDataSet.DefaultLostReportId, createdMessage.ReportId);
            Assert.Null(createdMessage.MatchId);
            Assert.False(createdMessage.Read);
            Assert.Null(createdMessage.ReadDate);

            createdMessageId = createdMessage.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Message_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdMessageId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageViewModel>(stringContent);

            Assert.NotNull(message);
            Assert.Equal(createdMessageId, message.Id);
            Assert.Equal(DefaultDataSet.DefaultUserId, message.SenderId);
            Assert.Equal(DefaultDataSet.DefaultRecipientUserId, message.RecipientId);
            Assert.Equal("Hello, I think I found your pet!", message.Content);
        }

        [Fact, TestPriority(4)]
        public async Task GetBySenderId_Returns_Messages_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/sender/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<MessageViewModel>>(stringContent);

            Assert.NotNull(messages);
            Assert.True(messages.Count > 0);
            Assert.Contains(messages, m => m.Id == createdMessageId);
        }

        [Fact, TestPriority(5)]
        public async Task GetByRecipientId_Returns_Messages_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/recipient/{DefaultDataSet.DefaultRecipientUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<MessageViewModel>>(stringContent);

            Assert.NotNull(messages);
            Assert.True(messages.Count > 0);
            Assert.Contains(messages, m => m.Id == createdMessageId);
        }

        [Fact, TestPriority(6)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var readDate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            var updateModel = new MessageUpdateModel(
                content: "Updated message content",
                read: true,
                readDate: readDate
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdMessageId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task GetById_Returns_Updated_Message_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdMessageId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageViewModel>(stringContent);

            Assert.NotNull(message);
            Assert.Equal(createdMessageId, message.Id);
            Assert.Equal("Updated message content", message.Content);
            Assert.True(message.Read);
            Assert.NotNull(message.ReadDate);
        }

        [Fact, TestPriority(8)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdMessageId}");

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
        public async Task Create_Returns_BadRequest_With_Missing_Required_Fields()
        {
            // Arrange
            var createModel = new
            {
                ReportId = DefaultDataSet.DefaultLostReportId
                // Missing SenderId, RecipientId, Content
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Content_Exceeding_MaxLength()
        {
            // Arrange
            var longContent = new string('A', 5001); // Exceeds 5000 character limit
            var createModel = new MessageCreateModel(
                senderId: DefaultDataSet.DefaultUserId,
                recipientId: DefaultDataSet.DefaultRecipientUserId,
                content: longContent,
                reportId: null,
                matchId: null
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
            var updateModel = new MessageUpdateModel(
                content: "Updated content",
                read: null,
                readDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{invalidId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_Content_Exceeding_MaxLength()
        {
            // Arrange - Create a message to update
            var createModel = new MessageCreateModel(
                senderId: DefaultDataSet.DefaultUserId,
                recipientId: DefaultDataSet.DefaultRecipientUserId,
                content: "Temporary message",
                reportId: null,
                matchId: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdMessage = JsonConvert.DeserializeObject<MessageViewModel>(createContent);

            // Arrange update with invalid content
            var longContent = new string('A', 5001); // Exceeds 5000 character limit
            var updateModel = new MessageUpdateModel(
                content: longContent,
                read: null,
                readDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdMessage!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdMessage.Id}");
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
        public async Task GetBySenderId_Returns_Empty_List_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/sender/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<MessageViewModel>>(stringContent);

            Assert.NotNull(messages);
            Assert.Empty(messages);
        }

        [Fact]
        public async Task GetByRecipientId_Returns_Empty_List_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/recipient/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<MessageViewModel>>(stringContent);

            Assert.NotNull(messages);
            Assert.Empty(messages);
        }

        #endregion
    }
}
