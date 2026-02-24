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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2026, 2, 24, 22, 1, 19, 464, DateTimeKind.Unspecified).AddTicks(467), new TimeSpan(0, 0, 0, 0, 0)), 1L, null, null, "sa@test.com", true, true, "Admmin", "Super", null, null, null, new byte[] { 190, 177, 99, 44, 149, 154, 30, 167, 62, 157, 69, 173, 137, 118, 80, 222, 14, 237, 156, 72, 41, 82, 60, 122, 185, 249, 36, 230, 136, 124, 181, 78, 27, 103, 60, 198, 156, 144, 147, 140, 209, 170, 65, 221, 225, 179, 222, 48, 24, 231, 109, 59, 21, 236, 43, 124, 193, 4, 66, 1, 195, 148, 94, 230, 234, 26, 123, 153, 140, 157, 69, 214, 133, 17, 67, 207, 12, 149, 165, 205, 197, 248, 51, 115, 184, 204, 210, 32, 172, 222, 41, 24, 136, 125, 113, 246, 160, 144, 21, 172, 193, 43, 249, 146, 158, 246, 164, 157, 16, 251, 190, 120, 207, 12, 174, 11, 205, 140, 30, 14, 129, 232, 234, 77, 164, 220, 234, 6, 141, 228, 116, 147, 193, 90, 26, 117, 109, 120, 8, 64, 148, 159, 144, 58, 50, 179, 36, 83, 20, 151, 123, 153, 29, 216, 3, 34, 65, 112, 115, 121, 140, 89, 244, 222, 13, 54, 50, 227, 210, 156, 183, 114, 124, 2, 53, 88, 64, 48, 65, 74, 117, 234, 53, 21, 86, 163, 73, 76, 101, 158, 42, 158, 8, 130, 52, 81, 149, 127, 90, 217, 158, 190, 37, 51, 245, 133, 41, 114, 125, 235, 167, 172, 132, 120, 41, 243, 165, 4, 78, 8, 217, 155, 130, 20, 210, 152, 224, 118, 87, 65, 191, 121, 47, 36, 69, 142, 202, 162, 8, 163, 187, 91, 144, 61, 53, 17, 138, 159, 96, 74, 220, 101, 148, 150, 234, 180 }, "000000000", null, "SuperAdmin", new byte[] { 232, 8, 80, 43, 199, 126, 2, 172, 208, 120, 144, 96, 53, 52, 57, 169, 248, 105, 24, 5, 217, 1, 138, 65, 93, 123, 114, 255, 7, 162, 63, 37, 189, 205, 183, 147, 241, 106, 199, 66, 185, 13, 85, 114, 152, 38, 76, 73, 51, 142, 157, 117, 15, 117, 207, 135, 240, 71, 233, 185, 201, 120, 67, 250, 247, 64, 163, 82, 49, 101, 105, 174, 244, 81, 2, 132, 9, 9, 1, 110, 159, 226, 28, 14, 208, 52, 20, 155, 11, 70, 220, 174, 143, 97, 80, 22, 181, 0, 219, 0, 16, 151, 170, 65, 218, 100, 217, 224, 111, 204, 36, 156, 198, 177, 115, 73, 174, 56, 97, 28, 66, 28, 172, 116, 172, 173, 61, 78 }, new DateTimeOffset(new DateTime(2026, 2, 24, 22, 1, 19, 464, DateTimeKind.Unspecified).AddTicks(467), new TimeSpan(0, 0, 0, 0, 0)), 0L });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAnimals_AdopterId",
                table: "AdoptionAnimals",
                column: "AdopterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAnimals_ShelterId",
                table: "AdoptionAnimals",
                column: "ShelterId");

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
                name: "VeterinaryAppointments");

            migrationBuilder.DropTable(
                name: "UserPets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
