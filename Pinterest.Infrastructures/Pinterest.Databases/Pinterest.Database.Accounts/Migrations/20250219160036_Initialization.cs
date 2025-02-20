using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinterest.Database.Accounts.Migrations
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
                name: "AccountInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    UserUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfo", x => x.Uuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInfo_Email",
                schema: "public",
                table: "AccountInfo",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInfo_Uuid",
                schema: "public",
                table: "AccountInfo",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountInfo",
                schema: "public");
        }
    }
}
