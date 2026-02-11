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

            migrationBuilder.CreateTable(
                name: "UserPets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "text", nullable: false),
                    Breed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    BirthDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApproximateAge = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalPhotosURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsNeutered = table.Column<bool>(type: "boolean", nullable: true),
                    Allergies = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MedicalNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_UserPets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "EmailVerified", "IsActive", "LastName", "Name", "OrganizationAddress", "OrganizationName", "OrganizationPhone", "Password", "PhoneNumber", "ProfilePhotoURL", "Role", "Salt", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2026, 2, 11, 18, 49, 51, 573, DateTimeKind.Unspecified).AddTicks(6752), new TimeSpan(0, 0, 0, 0, 0)), 1L, null, null, "sa@test.com", true, true, "Admmin", "Super", null, null, null, new byte[] { 22, 223, 6, 175, 6, 237, 131, 89, 247, 3, 227, 36, 100, 37, 223, 233, 203, 148, 38, 207, 40, 135, 179, 126, 236, 211, 100, 113, 162, 98, 98, 76, 152, 30, 0, 202, 142, 226, 253, 19, 187, 20, 89, 28, 119, 47, 89, 126, 81, 221, 16, 86, 77, 82, 107, 165, 188, 4, 201, 122, 169, 190, 136, 40, 150, 160, 14, 164, 40, 203, 46, 151, 193, 151, 80, 163, 244, 38, 169, 205, 50, 101, 9, 2, 207, 138, 242, 86, 191, 243, 206, 210, 65, 231, 108, 100, 141, 215, 131, 65, 34, 107, 233, 144, 37, 107, 76, 1, 98, 178, 174, 88, 2, 14, 249, 169, 89, 143, 142, 81, 210, 235, 31, 40, 167, 29, 116, 223, 122, 244, 255, 155, 50, 248, 223, 200, 42, 23, 215, 121, 103, 218, 58, 223, 128, 129, 117, 146, 3, 231, 189, 184, 19, 11, 45, 253, 237, 27, 210, 136, 241, 187, 16, 103, 118, 115, 58, 185, 148, 1, 49, 109, 165, 83, 16, 77, 234, 0, 71, 25, 112, 147, 185, 57, 220, 191, 27, 128, 186, 192, 241, 219, 203, 126, 220, 25, 214, 63, 186, 190, 195, 23, 41, 144, 59, 142, 151, 48, 225, 193, 25, 126, 251, 224, 239, 204, 177, 207, 138, 18, 52, 32, 119, 7, 165, 167, 218, 61, 253, 19, 40, 72, 148, 106, 167, 27, 70, 167, 210, 246, 185, 69, 84, 98, 254, 158, 169, 205, 34, 55, 196, 163, 126, 50, 79, 39 }, "000000000", null, "SuperAdmin", new byte[] { 96, 214, 217, 231, 53, 149, 247, 148, 10, 126, 221, 249, 168, 65, 141, 97, 116, 6, 216, 196, 156, 27, 13, 87, 119, 76, 220, 82, 155, 43, 223, 111, 144, 39, 188, 242, 99, 31, 113, 85, 116, 115, 215, 41, 134, 217, 170, 116, 25, 201, 49, 79, 247, 151, 229, 31, 198, 214, 57, 8, 31, 170, 183, 72, 216, 191, 20, 60, 226, 152, 220, 140, 75, 125, 154, 51, 143, 186, 12, 109, 195, 224, 70, 26, 56, 108, 194, 157, 231, 200, 71, 63, 16, 206, 55, 89, 11, 30, 27, 118, 197, 124, 140, 161, 36, 162, 12, 131, 217, 128, 184, 145, 230, 78, 216, 7, 96, 211, 75, 241, 95, 126, 110, 51, 97, 42, 85, 138 }, new DateTimeOffset(new DateTime(2026, 2, 11, 18, 49, 51, 573, DateTimeKind.Unspecified).AddTicks(6752), new TimeSpan(0, 0, 0, 0, 0)), 0L });

            migrationBuilder.CreateIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
