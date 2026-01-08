using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddedHeroIdToParsedMatchWardEff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeroId",
                table: "wards_parsed_matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_wards_wards_parsed_matches_MatchId_AccountId",
                table: "wards",
                columns: new[] { "MatchId", "AccountId" },
                principalTable: "wards_parsed_matches",
                principalColumns: new[] { "MatchId", "AccountId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_wards_wards_parsed_matches_MatchId_AccountId",
                table: "wards");

            migrationBuilder.DropColumn(
                name: "HeroId",
                table: "wards_parsed_matches");
        }
    }
}
