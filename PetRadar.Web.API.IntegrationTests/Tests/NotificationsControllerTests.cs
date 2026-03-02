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
    public class NotificationsControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/notifications";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdNotificationId = 0;

        public NotificationsControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Notifications_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(stringContent);

            Assert.NotNull(notifications);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var createModel = new NotificationCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                type: NotificationTypeEnum.Match,
                title: "New Match Found",
                message: "A potential match has been found for your lost pet report.",
                metadata: "{\"reportId\": 3000, \"matchScore\": 0.85}",
                deepLink: "/reports/3000/matches"
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdNotification = JsonConvert.DeserializeObject<NotificationViewModel>(stringContent);

            Assert.NotNull(createdNotification);
            Assert.True(createdNotification.Id > 0);
            Assert.Equal(DefaultDataSet.DefaultUserId, createdNotification.UserId);
            Assert.Equal(createModel.Type.ToString(), createdNotification.Type);
            Assert.Equal(createModel.Title, createdNotification.Title);
            Assert.Equal(createModel.Message, createdNotification.Message);
            Assert.Equal(createModel.Metadata, createdNotification.Metadata);
            Assert.Equal(createModel.DeepLink, createdNotification.DeepLink);
            Assert.False(createdNotification.Read);
            Assert.Null(createdNotification.ReadDate);

            createdNotificationId = createdNotification.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Notification_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdNotificationId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<NotificationViewModel>(stringContent);

            Assert.NotNull(notification);
            Assert.Equal(createdNotificationId, notification.Id);
            Assert.Equal(DefaultDataSet.DefaultUserId, notification.UserId);
            Assert.Equal("Match", notification.Type);
            Assert.Equal("New Match Found", notification.Title);
            Assert.Equal("A potential match has been found for your lost pet report.", notification.Message);
        }

        [Fact, TestPriority(4)]
        public async Task GetByUserId_Returns_Notifications_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/user/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(stringContent);

            Assert.NotNull(notifications);
            Assert.True(notifications.Count > 0);
            Assert.Contains(notifications, n => n.Id == createdNotificationId);
        }

        [Fact, TestPriority(5)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new NotificationUpdateModel(
                type: NotificationTypeEnum.System,
                title: "Updated Notification Title",
                message: "Updated notification message content.",
                metadata: "{\"updated\": true}",
                deepLink: "/updated/link",
                read: true,
                readDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdNotificationId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task GetById_Returns_Updated_Notification_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdNotificationId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<NotificationViewModel>(stringContent);

            Assert.NotNull(notification);
            Assert.Equal(createdNotificationId, notification.Id);
            Assert.Equal("System", notification.Type);
            Assert.Equal("Updated Notification Title", notification.Title);
            Assert.Equal("Updated notification message content.", notification.Message);
            Assert.Equal("{\"updated\": true}", notification.Metadata);
            Assert.Equal("/updated/link", notification.DeepLink);
            Assert.True(notification.Read);
            Assert.NotNull(notification.ReadDate);
        }

        [Fact, TestPriority(7)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdNotificationId}");

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
            var createModel = new NotificationCreateModel(
                userId: 999999,
                type: NotificationTypeEnum.System,
                title: "Test Title",
                message: "Test Message",
                metadata: null,
                deepLink: null
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
                // Missing Type, Title, Message
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
        public async Task Create_Returns_BadRequest_With_Title_Exceeding_MaxLength()
        {
            // Arrange
            var longTitle = new string('A', 256); // Exceeds 255 character limit
            var createModel = new NotificationCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                type: NotificationTypeEnum.System,
                title: longTitle,
                message: "Valid message",
                metadata: null,
                deepLink: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Message_Exceeding_MaxLength()
        {
            // Arrange
            var longMessage = new string('A', 501); // Exceeds 500 character limit
            var createModel = new NotificationCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                type: NotificationTypeEnum.System,
                title: "Valid Title",
                message: longMessage,
                metadata: null,
                deepLink: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_DeepLink_Exceeding_MaxLength()
        {
            // Arrange
            var longDeepLink = new string('A', 501); // Exceeds 500 character limit
            var createModel = new NotificationCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                type: NotificationTypeEnum.System,
                title: "Valid Title",
                message: "Valid Message",
                metadata: null,
                deepLink: longDeepLink
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
            var updateModel = new NotificationUpdateModel(
                type: null,
                title: "Updated Title",
                message: null,
                metadata: null,
                deepLink: null,
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
        public async Task Update_Returns_BadRequest_With_Title_Exceeding_MaxLength()
        {
            // Arrange - First create a notification to update
            var createModel = new NotificationCreateModel(
                userId: DefaultDataSet.DefaultUserId,
                type: NotificationTypeEnum.Message,
                title: "Temp Notification",
                message: "Temporary notification for testing",
                metadata: null,
                deepLink: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdNotification = JsonConvert.DeserializeObject<NotificationViewModel>(createContent);

            // Arrange update with invalid title
            var longTitle = new string('A', 256); // Exceeds 255 character limit
            var updateModel = new NotificationUpdateModel(
                type: null,
                title: longTitle,
                message: null,
                metadata: null,
                deepLink: null,
                read: null,
                readDate: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdNotification!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdNotification.Id}");
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
