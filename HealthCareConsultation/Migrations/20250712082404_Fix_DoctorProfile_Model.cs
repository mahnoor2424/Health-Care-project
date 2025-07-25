using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareConsultation.Migrations
{
    /// <inheritdoc />
    public partial class Fix_DoctorProfile_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AvailableTimeEnd",
                table: "DoctorProfiles",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DoctorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTimeEnd",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DoctorProfiles");
        }
    }
}
