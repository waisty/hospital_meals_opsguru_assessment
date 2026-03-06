using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Patient.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientMobileNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mobile_number",
                schema: "dbo",
                table: "patients",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            // Backfill existing rows with unique values
            migrationBuilder.Sql(@"
                UPDATE dbo.patients
                SET mobile_number = 'legacy-' || id::text
                WHERE mobile_number IS NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "mobile_number",
                schema: "dbo",
                table: "patients",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_patients_mobile_number",
                schema: "dbo",
                table: "patients",
                column: "mobile_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_patients_mobile_number",
                schema: "dbo",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "mobile_number",
                schema: "dbo",
                table: "patients");
        }
    }
}
