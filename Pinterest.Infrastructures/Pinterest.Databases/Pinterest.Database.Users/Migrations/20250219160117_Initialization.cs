using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinterest.Database.Users.Migrations
{
    /// <inheritdoc />
    public partial class Initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PhotoPath = table.Column<string>(type: "text", nullable: true),
                    UserThemes = table.Column<string>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePost",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    PostUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePost", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_FavoritePost_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_Subscription_User_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_User_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePost_UserId",
                schema: "public",
                table: "FavoritePost",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePost_Uuid",
                schema: "public",
                table: "FavoritePost",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_AuthorId",
                schema: "public",
                table: "Subscription",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriberId",
                schema: "public",
                table: "Subscription",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Uuid",
                schema: "public",
                table: "Subscription",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Uuid",
                schema: "public",
                table: "User",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePost",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Subscription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "User",
                schema: "public");
        }
    }
}
