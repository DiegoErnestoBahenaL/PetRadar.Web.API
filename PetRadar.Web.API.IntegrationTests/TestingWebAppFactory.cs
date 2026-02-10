using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetRadar.Core.Data;
using PetRadar.Web.API;
using PetRadar.Web.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Web.API.IntegrationTests
{
    public class TestingWebAppFactory : WebApplicationFactory<Program>
    {
        public TestingWebAppFactoryConfig Config { get; private set; } = new TestingWebAppFactoryConfig();

        private string jwtValue = string.Empty;

        public TestingWebAppFactory()
        {
            Environment.SetEnvironmentVariable(Constants.ASPNetcoreEnvironment, Constants.LocalIntegrationEnvironment);
            Environment.SetEnvironmentVariable(HostDefaults.EnvironmentKey, Constants.LocalIntegrationEnvironment);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            if (!string.IsNullOrEmpty(Config.ChangeEnvironmentTo))
            {
                builder.UseEnvironment(Config.ChangeEnvironmentTo);
            }

            builder.ConfigureServices(services =>
            {
                
                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();

                if (Config.InitializeDatabase)
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PetRadarDbContext>();

                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.Migrate();

                    Config.DataSet.SeedData(dbContext);
                }

                SetupJwtValue(scope);
            });
        }

        void SetupJwtValue(IServiceScope scope)
        {
            if (Config.IncludeAuthenticationHeader)
            {

                var tokenHelper = new JwtHelper();

                jwtValue = tokenHelper.GetToken(Config.DataSet.DefaultUserEntity).Token;
            }
        }
        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            if (Config.IncludeAuthenticationHeader)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtValue);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

        }
    }
}
