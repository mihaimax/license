using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifytimetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInterval",
                table: "TimeTables");

            migrationBuilder.RenameColumn(
                name: "Weekdays",
                table: "TimeTables",
                newName: "Weekday");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "TimeTables",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "TimeTables",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "TimeTables",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TimeTables");

            migrationBuilder.RenameColumn(
                name: "Weekday",
                table: "TimeTables",
                newName: "Weekdays");

            migrationBuilder.AddColumn<string>(
                name: "TimeInterval",
                table: "TimeTables",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
