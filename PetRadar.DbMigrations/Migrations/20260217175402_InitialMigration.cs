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
                values: new object[] { 1L, new DateTimeOffset(new DateTime(2026, 2, 17, 17, 54, 2, 76, DateTimeKind.Unspecified).AddTicks(535), new TimeSpan(0, 0, 0, 0, 0)), 1L, null, null, "sa@test.com", true, true, "Admmin", "Super", null, null, null, new byte[] { 14, 22, 111, 213, 164, 153, 70, 52, 101, 117, 196, 229, 2, 191, 251, 13, 92, 114, 160, 125, 91, 176, 88, 118, 77, 21, 146, 250, 253, 0, 138, 202, 67, 59, 156, 122, 96, 234, 73, 66, 20, 206, 11, 67, 110, 190, 247, 237, 169, 106, 106, 209, 134, 101, 152, 86, 198, 157, 224, 116, 252, 189, 235, 77, 18, 23, 37, 0, 58, 76, 83, 185, 95, 157, 139, 211, 70, 74, 148, 53, 214, 250, 1, 201, 117, 251, 146, 44, 159, 243, 186, 168, 196, 74, 207, 174, 163, 124, 236, 194, 41, 0, 126, 109, 22, 47, 58, 170, 150, 163, 172, 151, 187, 101, 133, 28, 170, 152, 111, 96, 150, 218, 231, 70, 92, 126, 219, 243, 72, 6, 132, 24, 134, 134, 111, 206, 212, 193, 66, 95, 104, 138, 109, 51, 64, 82, 211, 132, 135, 127, 123, 85, 122, 97, 60, 65, 227, 159, 59, 64, 169, 26, 32, 211, 71, 94, 169, 88, 213, 161, 242, 63, 2, 141, 35, 229, 24, 197, 99, 185, 81, 230, 246, 64, 233, 186, 250, 37, 1, 222, 132, 168, 228, 237, 247, 23, 92, 88, 51, 102, 8, 205, 49, 18, 141, 3, 185, 20, 233, 254, 74, 18, 207, 42, 1, 240, 202, 142, 229, 54, 120, 174, 138, 125, 153, 105, 134, 169, 54, 123, 168, 153, 146, 227, 94, 41, 212, 253, 39, 235, 233, 78, 23, 175, 36, 125, 211, 132, 169, 34, 83, 236, 219, 21, 48, 209 }, "000000000", null, "SuperAdmin", new byte[] { 229, 183, 112, 99, 219, 171, 145, 157, 124, 44, 79, 255, 60, 124, 254, 168, 205, 107, 162, 67, 7, 252, 162, 1, 185, 222, 177, 181, 208, 79, 242, 109, 118, 169, 44, 188, 93, 243, 180, 83, 124, 70, 254, 177, 143, 199, 194, 98, 2, 252, 229, 193, 132, 217, 5, 202, 70, 43, 157, 233, 159, 86, 170, 221, 34, 28, 244, 233, 237, 77, 83, 82, 54, 172, 3, 204, 27, 195, 102, 203, 46, 253, 5, 142, 70, 200, 14, 195, 81, 159, 207, 126, 48, 158, 177, 248, 38, 37, 27, 104, 13, 58, 193, 158, 216, 234, 242, 165, 196, 156, 227, 227, 241, 20, 18, 17, 104, 127, 52, 184, 159, 111, 71, 16, 150, 22, 80, 221 }, new DateTimeOffset(new DateTime(2026, 2, 17, 17, 54, 2, 76, DateTimeKind.Unspecified).AddTicks(535), new TimeSpan(0, 0, 0, 0, 0)), 0L });

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
                name: "VeterinaryAppointments");

            migrationBuilder.DropTable(
                name: "UserPets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
