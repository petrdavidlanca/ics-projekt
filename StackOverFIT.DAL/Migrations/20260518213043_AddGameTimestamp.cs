using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackOverFIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGameTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Libraries_Users_UserEntityId",
                table: "Libraries");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoGames_Libraries_LibraryEntityId",
                table: "VideoGames");

            migrationBuilder.DropIndex(
                name: "IX_VideoGames_LibraryEntityId",
                table: "VideoGames");

            migrationBuilder.DropColumn(
                name: "LibraryEntityId",
                table: "VideoGames");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VideoGames",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LibraryVideoGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LibraryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VideoGameId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryVideoGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryVideoGames_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryVideoGames_VideoGames_VideoGameId",
                        column: x => x.VideoGameId,
                        principalTable: "VideoGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryVideoGames_LibraryId",
                table: "LibraryVideoGames",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryVideoGames_VideoGameId",
                table: "LibraryVideoGames",
                column: "VideoGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Libraries_Users_UserEntityId",
                table: "Libraries",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Libraries_Users_UserEntityId",
                table: "Libraries");

            migrationBuilder.DropTable(
                name: "LibraryVideoGames");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VideoGames");

            migrationBuilder.AddColumn<Guid>(
                name: "LibraryEntityId",
                table: "VideoGames",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoGames_LibraryEntityId",
                table: "VideoGames",
                column: "LibraryEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Libraries_Users_UserEntityId",
                table: "Libraries",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoGames_Libraries_LibraryEntityId",
                table: "VideoGames",
                column: "LibraryEntityId",
                principalTable: "Libraries",
                principalColumn: "Id");
        }
    }
}
