using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mybad.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InitAddWardEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wards",
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
                    table.PrimaryKey("PK_Wards", x => new { x.MatchId, x.AccountId, x.PosX, x.PosY });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wards_AccountId",
                table: "Wards",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_MatchId",
                table: "Wards",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_MatchId_AccountId_PosX_PosY",
                table: "Wards",
                columns: new[] { "MatchId", "AccountId", "PosX", "PosY" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wards");
        }
    }
}
