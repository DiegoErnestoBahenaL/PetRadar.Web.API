using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using PetRadar.Common;
using PetRadar.Core.Data;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.DbMigrations
{
    public class CustomInitialMigration
    {
        public static void UpAfterAll(MigrationBuilder migrationBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var superAdminEmail = configuration["PetRadarDbContextSettings:SuperAdminEmail"];
            var superAdminPassword = configuration["PetRadarDbContextSettings:SuperAdminPassword"];

            var passwordHelper = new PasswordHelper();

            byte[] salt = passwordHelper.GenerateSalt();
            byte[] password = passwordHelper.GenerateHash(superAdminPassword, salt);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumns: new[] { "Id", "Role" },
                keyValues: new object[] { Constants.SuperAdminId, RoleEnum.SuperAdmin.ToString() },
                columns: new[] { "Password", "Salt", "Email" },
                values: new object[] { password, salt, superAdminEmail });
        }

        public static void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
