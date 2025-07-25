using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareConsultation.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedAtColumnToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Appointments");
        }
    }
}
