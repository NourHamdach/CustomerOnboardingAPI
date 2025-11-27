using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOTPAttemptForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_RegistrationProgress_ICNumber",
                table: "RegistrationProgress",
                column: "ICNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OTPAttempts_ICNumber",
                table: "OTPAttempts",
                column: "ICNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OTPAttempts_RegistrationProgress_ICNumber",
                table: "OTPAttempts",
                column: "ICNumber",
                principalTable: "RegistrationProgress",
                principalColumn: "ICNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OTPAttempts_RegistrationProgress_ICNumber",
                table: "OTPAttempts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_RegistrationProgress_ICNumber",
                table: "RegistrationProgress");

            migrationBuilder.DropIndex(
                name: "IX_OTPAttempts_ICNumber",
                table: "OTPAttempts");
        }
    }
}
