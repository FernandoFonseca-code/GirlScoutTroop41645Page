using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GirlScoutTroop41645Page.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToMemberIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SendEmail",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Scouts",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Scouts");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AspNetUsers",
                newName: "SendEmail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}
