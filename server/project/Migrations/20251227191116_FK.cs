using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Migrations
{
    /// <inheritdoc />
    public partial class FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
