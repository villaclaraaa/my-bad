using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerInfoInParsedMatchWardInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRadiantPlayer",
                table: "wards_parsed_matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWonMatch",
                table: "wards_parsed_matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRadiantPlayer",
                table: "wards_parsed_matches");

            migrationBuilder.DropColumn(
                name: "IsWonMatch",
                table: "wards_parsed_matches");
        }
    }
}
