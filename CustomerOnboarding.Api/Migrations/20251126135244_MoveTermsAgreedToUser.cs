using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveTermsAgreedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAgreed",
                table: "UserSecurity");

            migrationBuilder.AddColumn<bool>(
                name: "TermsAgreed",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAgreed",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "TermsAgreed",
                table: "UserSecurity",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
