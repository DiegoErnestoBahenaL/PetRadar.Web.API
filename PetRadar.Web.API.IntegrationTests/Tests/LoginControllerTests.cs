using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PetRadar.Web.API.IntegrationTests.DataSeeds;
using PetRadar.Web.API.IntegrationTests.Helpers;
using PetRadar.Web.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PetRadar.Web.API.IntegrationTests.Tests
{
    [TestCaseOrderer(PriorityOrderer.PriorityOrdererName, PriorityOrderer.PriorityOrdererAssemblyName)]

    public class LoginControllerTests : IClassFixture<TestingWebAppFactory>
    {
        readonly TestingWebAppFactory _factory;

        static string refreshTokenValue = string.Empty;
        static string tokenValue = string.Empty;

        public LoginControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;      
        }

        [Fact, TestPriority(1)]
        public async Task Login_Returns_Token_Succesfully()
        {
            // Arrange
            var loginModel = new LoginModel(DefaultDataSet.DefaultUserEmail, DefaultDataSet.DefaultUserPassword);

            var client = _factory.CreateClient();

            var jsonModel = JsonConvert.SerializeObject(loginModel);

            // Act
            var result = await client.PostAsync("/api/gate/login", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert 


            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);


            var stringContent = await result.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<UserTokenViewModel>(stringContent);

            Assert.NotNull(token);
            Assert.True(token.Token.Length > 0);

            if (token != null)
            {
                tokenValue = token.Token;
                refreshTokenValue = token.RefreshToken;
            }
        }
        [Fact, TestPriority(2)]
        public async Task RefreshToken_Returns_New_Token_Succesfully()
        {
            // Arrange
            var client = _factory.CreateClient();
            var refreshTokenModel = new RefreshTokenFromUiModel(refreshTokenValue); 


            var jsonModel = JsonConvert.SerializeObject(refreshTokenModel);
            // Act
            var result = await client.PostAsync("/api/gate/login/refresh", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<UserTokenViewModel>(stringContent);

            Assert.NotNull(token);
            Assert.True(token.Token.Length > 0);
        }
        
        [Fact, TestPriority(3)]
        public async Task  RefreshToken_Returns_BadRequest_With_Invalid_Token()
        {
            // Arrange
            var client = _factory.CreateClient();
            var refreshTokenModel = tokenValue;
            var jsonModel = JsonConvert.SerializeObject(refreshTokenModel);
            // Act
            var result = await client.PostAsync("/api/gate/login/refresh", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }
        [Fact]
        public async Task RefreshToken_Returns_BadRequest_With_Null_Token()
        {
            var client = _factory.CreateClient();
            // Arrange
            var refreshTokenModel = new RefreshTokenFromUiModel();

            var jsonModel = JsonConvert.SerializeObject(refreshTokenModel);

            // Act
            var result = await client.PostAsync("/api/gate/login/refresh", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));


            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Returns_Unauthorized_With_Invalid_Credentials()
        {
            // arrange
            var client = _factory.CreateClient();
            var loginModel = new LoginModel("notavalid@email.com", "notmypassword");

            var jsonModel = JsonConvert.SerializeObject(loginModel);

            // act

            var result = await client.PostAsync("/api/gate/login", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();

        }
    }
}
    