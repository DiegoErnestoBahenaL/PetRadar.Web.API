using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetRadar.DbMigrations.Migrations
{
    /// <inheritdoc />
    public partial class ColorBreedLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "UserPets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "UserPets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Reports",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "Reports",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "AdoptionAnimals",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "AdoptionAnimals",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Password", "Salt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 4, 15, 23, 35, 31, 87, DateTimeKind.Unspecified).AddTicks(278), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 238, 3, 166, 48, 159, 15, 66, 87, 163, 251, 219, 192, 118, 223, 109, 10, 30, 11, 129, 233, 80, 166, 67, 92, 57, 125, 170, 152, 123, 106, 55, 203, 179, 235, 121, 158, 39, 20, 40, 182, 11, 243, 246, 141, 90, 43, 216, 132, 14, 83, 154, 28, 34, 124, 222, 217, 210, 113, 84, 46, 165, 42, 0, 188, 79, 95, 224, 220, 156, 85, 143, 9, 110, 218, 49, 26, 41, 152, 153, 46, 142, 33, 168, 164, 138, 109, 5, 103, 22, 1, 197, 135, 166, 21, 53, 221, 35, 56, 13, 177, 230, 16, 92, 16, 127, 90, 217, 74, 195, 165, 6, 28, 243, 18, 237, 241, 156, 252, 246, 244, 251, 74, 20, 134, 201, 25, 57, 183, 193, 145, 188, 185, 68, 9, 174, 156, 152, 99, 225, 45, 100, 147, 68, 252, 52, 152, 100, 48, 7, 40, 122, 194, 22, 64, 240, 195, 7, 144, 79, 176, 138, 129, 237, 132, 32, 125, 128, 7, 136, 243, 131, 48, 13, 235, 2, 100, 233, 131, 225, 56, 136, 221, 219, 160, 185, 250, 229, 29, 228, 84, 150, 104, 182, 167, 176, 111, 226, 123, 128, 85, 48, 31, 92, 25, 63, 185, 223, 41, 7, 122, 95, 41, 249, 162, 104, 10, 168, 7, 146, 156, 209, 69, 140, 79, 96, 126, 106, 231, 51, 68, 223, 145, 26, 210, 96, 241, 115, 101, 83, 59, 83, 36, 213, 102, 226, 172, 123, 104, 37, 68, 226, 112, 180, 4, 42, 3 }, new byte[] { 154, 199, 17, 190, 153, 92, 29, 20, 255, 237, 236, 217, 145, 122, 66, 187, 117, 51, 192, 225, 138, 94, 30, 185, 78, 201, 37, 182, 30, 122, 157, 94, 229, 209, 16, 79, 130, 53, 159, 203, 119, 23, 101, 69, 94, 100, 101, 28, 150, 118, 167, 169, 105, 20, 141, 93, 199, 124, 232, 47, 244, 190, 9, 243, 19, 35, 245, 240, 197, 145, 105, 6, 144, 53, 21, 201, 153, 109, 200, 13, 140, 3, 55, 11, 87, 111, 5, 37, 9, 219, 162, 217, 245, 104, 138, 4, 38, 120, 176, 129, 119, 118, 131, 111, 73, 116, 153, 116, 224, 120, 119, 25, 127, 66, 38, 90, 16, 92, 239, 132, 150, 84, 55, 32, 121, 253, 3, 95 }, new DateTimeOffset(new DateTime(2026, 4, 15, 23, 35, 31, 87, DateTimeKind.Unspecified).AddTicks(278), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "UserPets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "UserPets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Reports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "Reports",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "AdoptionAnimals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                table: "AdoptionAnimals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Password", "Salt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 3, 2, 14, 19, 21, 1, DateTimeKind.Unspecified).AddTicks(7002), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 164, 193, 183, 4, 4, 17, 81, 155, 17, 176, 18, 20, 95, 52, 154, 245, 224, 211, 32, 167, 186, 15, 55, 70, 220, 163, 88, 15, 119, 159, 14, 55, 50, 230, 128, 142, 106, 42, 221, 167, 90, 196, 68, 239, 35, 8, 25, 116, 229, 161, 109, 55, 191, 141, 7, 81, 50, 204, 227, 117, 43, 102, 2, 87, 33, 134, 87, 30, 224, 131, 58, 221, 6, 246, 98, 224, 68, 126, 123, 114, 244, 26, 64, 59, 136, 15, 27, 89, 160, 127, 102, 89, 121, 188, 210, 226, 77, 126, 3, 185, 150, 26, 214, 80, 235, 107, 73, 201, 72, 48, 153, 173, 55, 12, 6, 154, 182, 17, 155, 71, 47, 39, 158, 112, 11, 81, 173, 75, 12, 68, 148, 143, 8, 146, 157, 179, 237, 183, 79, 173, 192, 230, 64, 177, 5, 71, 242, 207, 25, 5, 98, 250, 102, 122, 119, 51, 109, 235, 67, 122, 203, 158, 192, 196, 243, 28, 209, 48, 125, 89, 121, 98, 122, 55, 66, 53, 140, 136, 166, 32, 156, 45, 146, 6, 254, 161, 48, 136, 96, 56, 171, 125, 20, 215, 59, 153, 119, 209, 111, 97, 173, 31, 202, 191, 190, 140, 149, 7, 173, 163, 204, 102, 136, 74, 50, 207, 250, 49, 125, 176, 133, 51, 174, 13, 55, 28, 151, 142, 114, 204, 193, 74, 37, 205, 150, 225, 196, 203, 151, 61, 10, 96, 31, 229, 13, 149, 169, 126, 215, 187, 100, 162, 143, 156, 120, 15 }, new byte[] { 187, 232, 58, 40, 51, 124, 4, 209, 181, 231, 237, 220, 82, 15, 51, 52, 250, 173, 195, 124, 142, 4, 163, 209, 138, 147, 234, 223, 12, 101, 119, 15, 39, 65, 100, 118, 135, 254, 217, 164, 17, 32, 132, 5, 16, 19, 219, 71, 57, 80, 65, 62, 29, 21, 92, 173, 75, 37, 191, 230, 32, 141, 55, 248, 92, 173, 12, 174, 155, 231, 86, 129, 190, 71, 176, 243, 139, 77, 52, 81, 214, 245, 249, 35, 212, 97, 21, 218, 142, 59, 34, 45, 136, 86, 19, 185, 250, 249, 204, 150, 38, 235, 250, 3, 17, 10, 81, 22, 223, 47, 53, 109, 214, 59, 34, 180, 187, 246, 181, 115, 58, 94, 91, 27, 148, 26, 236, 237 }, new DateTimeOffset(new DateTime(2026, 3, 2, 14, 19, 21, 1, DateTimeKind.Unspecified).AddTicks(7002), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
