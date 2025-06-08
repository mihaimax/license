using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestrictDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Teachers_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Teachers_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Teachers_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Teachers_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
