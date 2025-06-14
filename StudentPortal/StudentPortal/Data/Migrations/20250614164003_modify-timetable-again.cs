using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifytimetableagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables",
                columns: new[] { "DepartmentCode", "Year", "Semester", "SubjectCode", "Weekday", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimeTables",
                table: "TimeTables",
                columns: new[] { "DepartmentCode", "Year", "Semester", "SubjectCode" });
        }
    }
}
