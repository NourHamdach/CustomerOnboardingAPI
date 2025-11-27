using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVerifiedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VerifiedEmail",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VerifiedMobile",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "OTPCode",
                table: "OTPAttempts",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "ICNumber",
                table: "OTPAttempts",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "OTPAttempts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerifiedMobile",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ICNumber",
                table: "OTPAttempts");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "OTPAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "OTPCode",
                table: "OTPAttempts",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4);
        }
    }
}
