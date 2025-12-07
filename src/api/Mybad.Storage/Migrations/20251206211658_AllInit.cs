using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AllInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matchup_allies",
                columns: table => new
                {
                    HeroId = table.Column<int>(type: "integer", nullable: false),
                    OtherHeroId = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matchup_allies", x => new { x.HeroId, x.OtherHeroId });
                });

            migrationBuilder.CreateTable(
                name: "matchup_checked_matches",
                columns: table => new
                {
                    MatchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matchup_checked_matches", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "matchup_enemies",
                columns: table => new
                {
                    HeroId = table.Column<int>(type: "integer", nullable: false),
                    OtherHeroId = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matchup_enemies", x => new { x.HeroId, x.OtherHeroId });
                });

            migrationBuilder.CreateTable(
                name: "wards",
                columns: table => new
                {
                    PosX = table.Column<int>(type: "integer", nullable: false),
                    PosY = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    TimeLivedSeconds = table.Column<int>(type: "integer", nullable: false),
                    WasDestroyed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards", x => new { x.MatchId, x.AccountId, x.PosX, x.PosY });
                });

            migrationBuilder.CreateTable(
                name: "wards_parsed_matches",
                columns: table => new
                {
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    PlayedAtDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wards_parsed_matches", x => new { x.MatchId, x.AccountId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_wards_AccountId",
                table: "wards",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_wards_MatchId",
                table: "wards",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_wards_MatchId_AccountId_PosX_PosY",
                table: "wards",
                columns: new[] { "MatchId", "AccountId", "PosX", "PosY" });

            migrationBuilder.CreateIndex(
                name: "IX_wards_parsed_matches_MatchId_AccountId",
                table: "wards_parsed_matches",
                columns: new[] { "MatchId", "AccountId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matchup_allies");

            migrationBuilder.DropTable(
                name: "matchup_checked_matches");

            migrationBuilder.DropTable(
                name: "matchup_enemies");

            migrationBuilder.DropTable(
                name: "wards");

            migrationBuilder.DropTable(
                name: "wards_parsed_matches");
        }
    }
}
