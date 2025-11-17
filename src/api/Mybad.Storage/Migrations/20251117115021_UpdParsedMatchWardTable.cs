using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class UpdParsedMatchWardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ParsedMatchWardInfos",
                table: "ParsedMatchWardInfos");

            migrationBuilder.AlterColumn<long>(
                name: "MatchId",
                table: "ParsedMatchWardInfos",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "ParsedMatchWardInfos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParsedMatchWardInfos",
                table: "ParsedMatchWardInfos",
                columns: new[] { "MatchId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_ParsedMatchWardInfos_MatchId_AccountId",
                table: "ParsedMatchWardInfos",
                columns: new[] { "MatchId", "AccountId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ParsedMatchWardInfos",
                table: "ParsedMatchWardInfos");

            migrationBuilder.DropIndex(
                name: "IX_ParsedMatchWardInfos_MatchId_AccountId",
                table: "ParsedMatchWardInfos");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ParsedMatchWardInfos");

            migrationBuilder.AlterColumn<long>(
                name: "MatchId",
                table: "ParsedMatchWardInfos",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParsedMatchWardInfos",
                table: "ParsedMatchWardInfos",
                column: "MatchId");
        }
    }
}
