using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinterest.Database.Posts.Migrations
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
                name: "PostInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AuthorUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FileUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviewUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    ViewsCount = table.Column<int>(type: "integer", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    CommentsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    GrantedAccess = table.Column<string>(type: "json", nullable: false),
                    Tags = table.Column<string>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInfo", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_Comment_Comment_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalSchema: "public",
                        principalTable: "Comment",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Comment_PostInfo_PostId",
                        column: x => x.PostId,
                        principalSchema: "public",
                        principalTable: "PostInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentCommentId",
                schema: "public",
                table: "Comment",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                schema: "public",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Uuid",
                schema: "public",
                table: "Comment",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostInfo_Uuid",
                schema: "public",
                table: "PostInfo",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PostInfo",
                schema: "public");
        }
    }
}
