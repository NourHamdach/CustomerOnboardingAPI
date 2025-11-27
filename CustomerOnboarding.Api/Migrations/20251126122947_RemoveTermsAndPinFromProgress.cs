using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTermsAndPinFromProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinSet",
                table: "RegistrationProgress");

            migrationBuilder.DropColumn(
                name: "TermsAccepted",
                table: "RegistrationProgress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PinSet",
                table: "RegistrationProgress",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAccepted",
                table: "RegistrationProgress",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
