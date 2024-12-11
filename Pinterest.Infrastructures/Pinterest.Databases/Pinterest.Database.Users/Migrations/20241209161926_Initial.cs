using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinterest.Database.Users.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PhotoPath = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePosts",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    PostUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    UserUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePosts", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_FavoritePosts_User_UserUuid",
                        column: x => x.UserUuid,
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
                    SubscriptionsUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriberUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_Subscription_User_SubscriberUuid",
                        column: x => x.SubscriberUuid,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_User_SubscriptionsUuid",
                        column: x => x.SubscriptionsUuid,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserThemes",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserThemes", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_UserThemes_User_UserUuid",
                        column: x => x.UserUuid,
                        principalSchema: "public",
                        principalTable: "User",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePosts_UserUuid",
                table: "FavoritePosts",
                column: "UserUuid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriberUuid",
                schema: "public",
                table: "Subscription",
                column: "SubscriberUuid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriptionsUuid",
                schema: "public",
                table: "Subscription",
                column: "SubscriptionsUuid");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Uuid",
                schema: "public",
                table: "Subscription",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "public",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Uuid",
                schema: "public",
                table: "User",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserThemes_UserUuid",
                table: "UserThemes",
                column: "UserUuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePosts");

            migrationBuilder.DropTable(
                name: "Subscription",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserThemes");

            migrationBuilder.DropTable(
                name: "User",
                schema: "public");
        }
    }
}
