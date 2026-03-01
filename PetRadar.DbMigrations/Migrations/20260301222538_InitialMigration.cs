using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
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
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

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
                name: "AdoptionAnimals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ShelterId = table.Column<long>(type: "bigint", nullable: false),
                    Personality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GoodWithKids = table.Column<bool>(type: "boolean", nullable: true),
                    GoodWithDogs = table.Column<bool>(type: "boolean", nullable: true),
                    GoodWithCats = table.Column<bool>(type: "boolean", nullable: true),
                    IsVaccinated = table.Column<bool>(type: "boolean", nullable: true),
                    NeedsSpecialCare = table.Column<bool>(type: "boolean", nullable: true),
                    SpecialCareDetails = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AdoptionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AdopterId = table.Column<long>(type: "bigint", nullable: true),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Species = table.Column<string>(type: "text", nullable: false),
                    Breed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    ApproximateAge = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalPhotosURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsNeutered = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAnimals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionAnimals_Users_AdopterId",
                        column: x => x.AdopterId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdoptionAnimals_Users_ShelterId",
                        column: x => x.ShelterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BirthDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Allergies = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MedicalNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Species = table.Column<string>(type: "text", nullable: false),
                    Breed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    ApproximateAge = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalPhotosURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsNeutered = table.Column<bool>(type: "boolean", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserPetId = table.Column<long>(type: "bigint", nullable: true),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    ReportStatus = table.Column<string>(type: "text", nullable: false),
                    HasCollar = table.Column<bool>(type: "boolean", nullable: true),
                    HasTag = table.Column<bool>(type: "boolean", nullable: true),
                    ReportDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IncidentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false),
                    AddressText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SearchRadiusMeters = table.Column<int>(type: "integer", nullable: false),
                    UseAlternateContact = table.Column<bool>(type: "boolean", nullable: false),
                    ContactName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OffersReward = table.Column<bool>(type: "boolean", nullable: false),
                    RewardAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Views = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Species = table.Column<string>(type: "text", nullable: false),
                    Breed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    ApproximateAge = table.Column<decimal>(type: "numeric", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalPhotosURL = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsNeutered = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_UserPets_UserPetId",
                        column: x => x.UserPetId,
                        principalTable: "UserPets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VeterinaryAppointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<long>(type: "bigint", nullable: false),
                    VeterinaryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AppointmentType = table.Column<string>(type: "text", nullable: false),
                    AppointmentStatus = table.Column<string>(type: "text", nullable: false),
                    AppointmentDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: true),
                    ReasonForVisit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Diagnosis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Treatment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Prescriptions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true),
                    Location = table.Column<Point>(type: "geography (point)", nullable: true),
                    AddressText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReminderSent = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_VeterinaryAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeterinaryAppointments_UserPets_PetId",
                        column: x => x.PetId,
                        principalTable: "UserPets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "EmailVerified", "IsActive", "LastName", "Name", "OrganizationAddress", "OrganizationName", "OrganizationPhone", "Password", "PhoneNumber", "ProfilePhotoURL", "Role", "Salt", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2026, 3, 1, 22, 25, 38, 333, DateTimeKind.Unspecified).AddTicks(8383), new TimeSpan(0, 0, 0, 0, 0)), 1L, null, null, "sa@test.com", true, true, "Admmin", "Super", null, null, null, new byte[] { 6, 132, 253, 77, 33, 227, 169, 130, 189, 102, 158, 242, 57, 161, 204, 173, 188, 194, 243, 136, 213, 188, 84, 68, 179, 211, 238, 185, 45, 154, 55, 120, 234, 183, 150, 170, 33, 98, 250, 222, 61, 229, 240, 195, 89, 103, 114, 231, 203, 225, 53, 157, 200, 109, 245, 69, 112, 162, 114, 12, 150, 189, 102, 152, 199, 110, 135, 188, 116, 27, 40, 169, 2, 217, 238, 35, 48, 128, 0, 73, 149, 193, 132, 141, 227, 195, 169, 114, 104, 222, 236, 41, 208, 210, 112, 97, 103, 236, 100, 199, 107, 74, 106, 36, 66, 88, 65, 34, 146, 0, 191, 45, 136, 105, 143, 96, 56, 69, 173, 121, 254, 200, 44, 79, 248, 207, 128, 95, 89, 83, 171, 130, 200, 154, 25, 173, 227, 42, 159, 93, 170, 245, 14, 201, 63, 47, 211, 64, 69, 11, 192, 100, 243, 239, 218, 191, 109, 104, 31, 124, 212, 238, 106, 18, 21, 32, 85, 252, 58, 58, 77, 84, 95, 56, 116, 61, 248, 111, 91, 157, 209, 76, 206, 133, 57, 181, 226, 68, 209, 241, 147, 91, 89, 168, 183, 142, 124, 191, 123, 54, 175, 103, 47, 67, 38, 69, 49, 253, 199, 231, 133, 233, 253, 48, 69, 62, 110, 178, 98, 25, 234, 145, 239, 55, 79, 72, 38, 177, 94, 0, 228, 126, 70, 102, 162, 149, 78, 252, 209, 235, 169, 211, 16, 214, 212, 37, 244, 161, 215, 193, 18, 133, 48, 84, 148, 240 }, "000000000", null, "SuperAdmin", new byte[] { 55, 137, 144, 223, 82, 114, 251, 128, 249, 253, 197, 74, 157, 146, 158, 186, 248, 164, 34, 84, 106, 79, 176, 6, 194, 20, 234, 183, 212, 187, 198, 173, 243, 153, 185, 110, 210, 156, 240, 12, 109, 27, 79, 97, 177, 114, 130, 255, 110, 157, 167, 137, 5, 214, 37, 73, 168, 241, 246, 173, 195, 31, 86, 193, 114, 138, 126, 222, 168, 33, 190, 185, 158, 9, 164, 212, 162, 168, 58, 35, 50, 94, 83, 41, 187, 28, 24, 133, 246, 253, 15, 187, 25, 223, 246, 119, 188, 18, 120, 140, 27, 79, 199, 151, 114, 71, 15, 208, 104, 55, 42, 30, 155, 115, 114, 182, 57, 58, 160, 125, 21, 5, 68, 6, 24, 158, 49, 68 }, new DateTimeOffset(new DateTime(2026, 3, 1, 22, 25, 38, 333, DateTimeKind.Unspecified).AddTicks(8383), new TimeSpan(0, 0, 0, 0, 0)), 0L });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAnimals_AdopterId",
                table: "AdoptionAnimals",
                column: "AdopterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAnimals_ShelterId",
                table: "AdoptionAnimals",
                column: "ShelterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserPetId",
                table: "Reports",
                column: "UserPetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VeterinaryAppointments_PetId",
                table: "VeterinaryAppointments",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionAnimals");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "VeterinaryAppointments");

            migrationBuilder.DropTable(
                name: "UserPets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
