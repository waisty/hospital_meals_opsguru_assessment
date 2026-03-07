using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Patient.Core.Migrations
{
    /// <inheritdoc />
    public partial class PatientFirstNameLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            // Backfill: split name on first space; first part = first_name, rest = last_name
            migrationBuilder.Sql(@"
                UPDATE dbo.patients
                SET
                    first_name = CASE
                        WHEN position(' ' IN TRIM(name)) > 0 THEN TRIM(LEFT(TRIM(name), position(' ' IN TRIM(name)) - 1))
                        ELSE TRIM(name)
                    END,
                    last_name = CASE
                        WHEN position(' ' IN TRIM(name)) > 0 THEN TRIM(SUBSTRING(TRIM(name) FROM position(' ' IN TRIM(name)) + 1))
                        ELSE ''
                    END
                WHERE first_name IS NULL OR last_name IS NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "name",
                schema: "dbo",
                table: "patients");

            migrationBuilder.CreateIndex(
                name: "IX_patients_first_name",
                schema: "dbo",
                table: "patients",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "IX_patients_last_name",
                schema: "dbo",
                table: "patients",
                column: "last_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_patients_first_name",
                schema: "dbo",
                table: "patients");

            migrationBuilder.DropIndex(
                name: "IX_patients_last_name",
                schema: "dbo",
                table: "patients");

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE dbo.patients
                SET name = TRIM(COALESCE(first_name, '') || ' ' || COALESCE(last_name, ''))
                WHERE name IS NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "dbo",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "dbo",
                table: "patients");
        }
    }
}
