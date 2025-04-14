using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirlScoutTroop41645Page.Data.Migrations
{
    /// <inheritdoc />
    public partial class linktables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberScout",
                columns: table => new
                {
                    MembersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScoutsScoutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberScout", x => new { x.MembersId, x.ScoutsScoutId });
                    table.ForeignKey(
                        name: "FK_MemberScout_AspNetUsers_MembersId",
                        column: x => x.MembersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberScout_Scouts_ScoutsScoutId",
                        column: x => x.ScoutsScoutId,
                        principalTable: "Scouts",
                        principalColumn: "ScoutId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberScout_ScoutsScoutId",
                table: "MemberScout",
                column: "ScoutsScoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberScout");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "AspNetUsers");
        }
    }
}
