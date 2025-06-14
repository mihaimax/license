using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class da : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseTeacherId",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LabTeacherId",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CourseTeacherId",
                table: "Subjects",
                column: "CourseTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_LabTeacherId",
                table: "Subjects",
                column: "LabTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_CourseTeacherId",
                table: "Subjects",
                column: "CourseTeacherId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_LabTeacherId",
                table: "Subjects",
                column: "LabTeacherId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_CourseTeacherId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_LabTeacherId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_CourseTeacherId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_LabTeacherId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "CourseTeacherId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "LabTeacherId",
                table: "Subjects");
        }
    }
}
