using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_matchup_enemies",
                table: "matchup_enemies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_matchup_allies",
                table: "matchup_allies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heroes_matches_counts",
                table: "heroes_matches_counts");

            migrationBuilder.AddColumn<int>(
                name: "PatchId",
                table: "matchup_enemies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatchId",
                table: "matchup_checked_matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatchId",
                table: "matchup_allies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HeroId",
                table: "heroes_matches_counts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "PatchId",
                table: "heroes_matches_counts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchup_enemies",
                table: "matchup_enemies",
                columns: new[] { "HeroId", "OtherHeroId", "PatchId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchup_allies",
                table: "matchup_allies",
                columns: new[] { "HeroId", "OtherHeroId", "PatchId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_heroes_matches_counts",
                table: "heroes_matches_counts",
                columns: new[] { "HeroId", "PatchId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_matchup_enemies",
                table: "matchup_enemies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_matchup_allies",
                table: "matchup_allies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heroes_matches_counts",
                table: "heroes_matches_counts");

            migrationBuilder.DropColumn(
                name: "PatchId",
                table: "matchup_enemies");

            migrationBuilder.DropColumn(
                name: "PatchId",
                table: "matchup_checked_matches");

            migrationBuilder.DropColumn(
                name: "PatchId",
                table: "matchup_allies");

            migrationBuilder.DropColumn(
                name: "PatchId",
                table: "heroes_matches_counts");

            migrationBuilder.AlterColumn<int>(
                name: "HeroId",
                table: "heroes_matches_counts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchup_enemies",
                table: "matchup_enemies",
                columns: new[] { "HeroId", "OtherHeroId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchup_allies",
                table: "matchup_allies",
                columns: new[] { "HeroId", "OtherHeroId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_heroes_matches_counts",
                table: "heroes_matches_counts",
                column: "HeroId");
        }
    }
}
