using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackOverFIT.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CurrentState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    real_name = table.Column<string>(type: "TEXT", nullable: true),
                    surname = table.Column<string>(type: "TEXT", nullable: true),
                    total_num_of_games = table.Column<int>(type: "INTEGER", nullable: false),
                    total_num_of_libraries = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "libraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    num_of_games = table.Column<int>(type: "INTEGER", nullable: false),
                    user_entityId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_libraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_libraries_users_user_entityId",
                        column: x => x.user_entityId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "video_games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    pegi_rating = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    game_genres = table.Column<string>(type: "TEXT", nullable: false),
                    library_entityId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_video_games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_video_games_libraries_library_entityId",
                        column: x => x.library_entityId,
                        principalTable: "libraries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_libraries_user_entityId",
                table: "libraries",
                column: "user_entityId");

            migrationBuilder.CreateIndex(
                name: "IX_video_games_library_entityId",
                table: "video_games",
                column: "library_entityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "video_games");

            migrationBuilder.DropTable(
                name: "libraries");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
