using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastSeenIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastSeenAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_sessions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "refresh_tokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                INSERT INTO user_sessions (
                    "Id",
                    "UserId",
                    "DeviceName",
                    "UserAgent",
                    "CreatedByIp",
                    "LastSeenIp",
                    "LastSeenAtUtc",
                    "RevokedAtUtc",
                    "CreatedAtUtc",
                    "UpdatedAtUtc")
                SELECT
                    "Id",
                    "UserId",
                    'Unknown device',
                    NULL,
                    "CreatedByIp",
                    "CreatedByIp",
                    "CreatedAtUtc",
                    "RevokedAtUtc",
                    "CreatedAtUtc",
                    "UpdatedAtUtc"
                FROM refresh_tokens;
                """);

            migrationBuilder.Sql(
                """
                UPDATE refresh_tokens
                SET "SessionId" = "Id";
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "refresh_tokens",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_SessionId",
                table: "refresh_tokens",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_UserId",
                table: "user_sessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_user_sessions_SessionId",
                table: "refresh_tokens",
                column: "SessionId",
                principalTable: "user_sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_user_sessions_SessionId",
                table: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_SessionId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "refresh_tokens");
        }
    }
}
