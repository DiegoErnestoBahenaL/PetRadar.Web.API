using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetRadar.DbMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Added_FcmToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FcmToken",
                table: "Users",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "FcmToken",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmToken",
                table: "Users");
        }
    }
}
