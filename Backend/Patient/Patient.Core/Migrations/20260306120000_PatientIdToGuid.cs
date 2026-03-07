using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Patient.Core.Migrations
{
    /// <inheritdoc />
    public partial class PatientIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.DropTable(
                name: "patient_allergies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "patient_clinical_states",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_patients",
                schema: "dbo",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "dbo",
                table: "patients");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "dbo",
                table: "patients",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_patients",
                schema: "dbo",
                table: "patients",
                column: "id");

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
                name: "IX_patient_allergies_allergy_id",
                schema: "dbo",
                table: "patient_allergies",
                column: "allergy_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_clinical_states_clinical_state_id",
                schema: "dbo",
                table: "patient_clinical_states",
                column: "clinical_state_id");
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_patients",
                schema: "dbo",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "dbo",
                table: "patients");

            migrationBuilder.AddColumn<string>(
                name: "id",
                schema: "dbo",
                table: "patients",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_patients",
                schema: "dbo",
                table: "patients",
                column: "id");

            migrationBuilder.CreateTable(
                name: "patient_allergies",
                schema: "dbo",
                columns: table => new
                {
                    patient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                    patient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                name: "IX_patient_allergies_allergy_id",
                schema: "dbo",
                table: "patient_allergies",
                column: "allergy_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_clinical_states_clinical_state_id",
                schema: "dbo",
                table: "patient_clinical_states",
                column: "clinical_state_id");
        }
    }
}
