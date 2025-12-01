using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PetRadar.DbMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", maxLength: 256, nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProfilePhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    OrganizationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OrganizationAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OrganizationPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "EmailVerified", "IsActive", "LastName", "Name", "OrganizationAddress", "OrganizationName", "OrganizationPhone", "Password", "PhoneNumber", "ProfilePhotoURL", "Role", "Salt", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2025, 12, 1, 16, 47, 40, 766, DateTimeKind.Unspecified).AddTicks(4013), new TimeSpan(0, 0, 0, 0, 0)), 1L, null, null, "sa@test.com", true, true, "Admmin", "Super", null, null, null, new byte[] { 44, 250, 17, 237, 148, 114, 50, 65, 149, 20, 24, 67, 211, 83, 95, 17, 116, 46, 171, 11, 161, 166, 226, 186, 19, 213, 60, 244, 53, 110, 185, 130, 106, 19, 185, 109, 19, 248, 208, 86, 247, 208, 195, 126, 226, 243, 106, 254, 83, 23, 73, 32, 85, 133, 97, 91, 243, 249, 227, 39, 103, 159, 217, 143, 210, 39, 159, 181, 56, 236, 22, 230, 142, 163, 4, 12, 242, 118, 194, 32, 201, 177, 66, 97, 98, 96, 147, 23, 68, 93, 171, 44, 156, 183, 70, 52, 200, 122, 79, 199, 142, 77, 215, 206, 166, 9, 174, 237, 216, 251, 234, 231, 179, 193, 154, 148, 121, 214, 141, 215, 100, 205, 52, 152, 203, 149, 46, 52, 93, 48, 11, 72, 177, 223, 13, 84, 228, 44, 218, 32, 113, 90, 18, 233, 184, 87, 176, 135, 98, 82, 45, 151, 52, 178, 213, 243, 112, 223, 3, 59, 252, 240, 196, 67, 255, 1, 65, 75, 114, 151, 9, 179, 237, 147, 53, 242, 14, 235, 136, 165, 220, 128, 220, 244, 200, 203, 28, 172, 251, 22, 140, 255, 81, 36, 112, 52, 63, 162, 48, 212, 75, 237, 250, 41, 22, 116, 223, 9, 164, 114, 219, 203, 243, 155, 154, 83, 25, 219, 67, 110, 197, 88, 210, 24, 221, 150, 198, 108, 48, 165, 205, 80, 195, 24, 203, 92, 19, 3, 170, 199, 1, 58, 104, 27, 145, 87, 93, 244, 124, 174, 251, 240, 50, 227, 83, 80 }, "000000000", null, "SuperAdmin", new byte[] { 189, 72, 68, 182, 63, 21, 246, 178, 107, 179, 23, 72, 201, 63, 136, 53, 30, 89, 37, 116, 233, 195, 85, 70, 60, 127, 127, 160, 242, 226, 161, 137, 103, 151, 79, 229, 1, 25, 87, 43, 9, 171, 113, 159, 96, 109, 23, 83, 113, 192, 9, 128, 42, 51, 217, 120, 239, 165, 209, 211, 0, 223, 223, 116, 93, 69, 126, 53, 117, 23, 14, 134, 158, 252, 168, 248, 246, 237, 7, 167, 86, 94, 21, 178, 104, 158, 169, 110, 104, 52, 32, 173, 82, 220, 56, 71, 184, 121, 53, 175, 182, 146, 141, 202, 7, 20, 22, 34, 32, 202, 25, 53, 123, 137, 49, 255, 251, 155, 176, 7, 17, 249, 71, 63, 72, 108, 216, 175 }, new DateTimeOffset(new DateTime(2025, 12, 1, 16, 47, 40, 766, DateTimeKind.Unspecified).AddTicks(4013), new TimeSpan(0, 0, 0, 0, 0)), 0L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
