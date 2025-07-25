using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareConsultation.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressAndAgeToPatientProfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PatientProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "PatientProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "PatientProfiles");
        }
    }
}
