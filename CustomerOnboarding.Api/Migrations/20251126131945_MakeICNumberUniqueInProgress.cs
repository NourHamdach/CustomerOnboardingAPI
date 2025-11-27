using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboarding.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeICNumberUniqueInProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RegistrationProgress_ICNumber",
                table: "RegistrationProgress",
                column: "ICNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationProgress_ICNumber",
                table: "RegistrationProgress");
        }
    }
}
