using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToUserBasedOTPv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationProgress");

            migrationBuilder.DropColumn(
                name: "ICNumber",
                table: "OTPAttempts");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Flow",
                table: "OTPAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "OTPAttempts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OTPAttempts_UserId",
                table: "OTPAttempts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OTPAttempts_Users_UserId",
                table: "OTPAttempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OTPAttempts_Users_UserId",
                table: "OTPAttempts");

            migrationBuilder.DropIndex(
                name: "IX_OTPAttempts_UserId",
                table: "OTPAttempts");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Flow",
                table: "OTPAttempts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OTPAttempts");

            migrationBuilder.AddColumn<string>(
                name: "ICNumber",
                table: "OTPAttempts",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RegistrationProgress",
                columns: table => new
                {
                    ProgressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    ICNumber = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MobileVerified = table.Column<bool>(type: "boolean", nullable: false),
                    TempEmail = table.Column<string>(type: "text", nullable: true),
                    TempMobile = table.Column<string>(type: "text", nullable: true),
                    TempName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationProgress", x => x.ProgressId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationProgress_ICNumber",
                table: "RegistrationProgress",
                column: "ICNumber",
                unique: true);
        }
    }
}
