using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirlScoutTroop41645Page.Data.Migrations
{
    /// <inheritdoc />
    public partial class mssqlazure_migration_138 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
