using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations
{
    /// <inheritdoc />
    public partial class AddFildesForGift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Gifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Gifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Gifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SponsorId",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_SponsorId",
                table: "Gifts",
                column: "SponsorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Sponsors_SponsorId",
                table: "Gifts",
                column: "SponsorId",
                principalTable: "Sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Sponsors_SponsorId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_SponsorId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "SponsorId",
                table: "Gifts");
        }
    }
}
