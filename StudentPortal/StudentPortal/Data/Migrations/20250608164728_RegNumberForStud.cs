using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegNumberForStud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Students");
        }
    }
}
