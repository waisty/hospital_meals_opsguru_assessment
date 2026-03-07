using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Patient.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "allergies",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clinical_states",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinical_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "diet_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diet_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    first_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    last_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    mobile_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    diet_type_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                    table.ForeignKey(
                        name: "FK_patients_diet_types_diet_type_id",
                        column: x => x.diet_type_id,
                        principalSchema: "dbo",
                        principalTable: "diet_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_allergies",
                schema: "dbo",
                columns: table => new
                {
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allergy_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_allergies", x => new { x.patient_id, x.allergy_id });
                    table.ForeignKey(
                        name: "FK_patient_allergies_allergies_allergy_id",
                        column: x => x.allergy_id,
                        principalSchema: "dbo",
                        principalTable: "allergies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_allergies_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "dbo",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_clinical_states",
                schema: "dbo",
                columns: table => new
                {
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinical_state_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_clinical_states", x => new { x.patient_id, x.clinical_state_id });
                    table.ForeignKey(
                        name: "FK_patient_clinical_states_clinical_states_clinical_state_id",
                        column: x => x.clinical_state_id,
                        principalSchema: "dbo",
                        principalTable: "clinical_states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_clinical_states_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "dbo",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allergies_name",
                schema: "dbo",
                table: "allergies",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clinical_states_name",
                schema: "dbo",
                table: "clinical_states",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_diet_types_name",
                schema: "dbo",
                table: "diet_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_allergies_allergy_id",
                schema: "dbo",
                table: "patient_allergies",
                column: "allergy_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_clinical_states_clinical_state_id",
                schema: "dbo",
                table: "patient_clinical_states",
                column: "clinical_state_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_diet_type_id",
                schema: "dbo",
                table: "patients",
                column: "diet_type_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_patients_middle_name",
                schema: "dbo",
                table: "patients",
                column: "middle_name");

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
            migrationBuilder.DropTable(
                name: "patient_allergies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "patient_clinical_states",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "allergies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "clinical_states",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "patients",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "diet_types",
                schema: "dbo");
        }
    }
}
