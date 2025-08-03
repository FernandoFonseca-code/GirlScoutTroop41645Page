using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirlScoutTroop41645Page.Migrations
{
    /// <inheritdoc />
    public partial class AddTroopLevelSubcategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TroopLevelSubcategories",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TroopLevelSubcategories",
                table: "AspNetUsers");
        }
    }
}
