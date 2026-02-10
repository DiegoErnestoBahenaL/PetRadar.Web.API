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

        static string refreshTokenValue;
        static string tokenValue;

        public LoginControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
        }

        [Fact, TestPriority(1)]
        public async Task Login_Returns_Tokens()
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


            var str = await result.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<UserTokenViewModel>(str);

            tokenValue = token.Token;
            refreshTokenValue = token.RefreshToken;
            Assert.NotNull(token);
            Assert.True(token.Token.Length > 0);
        }
    }
}
    