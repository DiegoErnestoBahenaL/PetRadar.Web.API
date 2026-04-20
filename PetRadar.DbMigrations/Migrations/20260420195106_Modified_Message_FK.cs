using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetRadar.DbMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Modified_Message_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Reports_ReportId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "Messages",
                newName: "AdoptionAnimalId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReportId",
                table: "Messages",
                newName: "IX_Messages_AdoptionAnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AdoptionAnimals_AdoptionAnimalId",
                table: "Messages",
                column: "AdoptionAnimalId",
                principalTable: "AdoptionAnimals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AdoptionAnimals_AdoptionAnimalId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "AdoptionAnimalId",
                table: "Messages",
                newName: "ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_AdoptionAnimalId",
                table: "Messages",
                newName: "IX_Messages_ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Reports_ReportId",
                table: "Messages",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id");
        }
    }
}
