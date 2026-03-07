using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Kitchen.Core.Migrations
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
                name: "trays",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    patient_meal_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    first_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    last_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    recipe_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    received_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trays", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tray_ingredients",
                schema: "dbo",
                columns: table => new
                {
                    tray_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingredient_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    qty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tray_ingredients", x => new { x.tray_id, x.ingredient_name });
                    table.ForeignKey(
                        name: "FK_tray_ingredients_trays_tray_id",
                        column: x => x.tray_id,
                        principalSchema: "dbo",
                        principalTable: "trays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tray_status_history",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tray_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tray_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_tray_status_history_trays_tray_id",
                        column: x => x.tray_id,
                        principalSchema: "dbo",
                        principalTable: "trays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tray_status_history_tray_id",
                schema: "dbo",
                table: "tray_status_history",
                column: "tray_id");

            migrationBuilder.CreateIndex(
                name: "IX_trays_first_name",
                schema: "dbo",
                table: "trays",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "IX_trays_last_name",
                schema: "dbo",
                table: "trays",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "IX_trays_middle_name",
                schema: "dbo",
                table: "trays",
                column: "middle_name");

            migrationBuilder.CreateIndex(
                name: "IX_trays_patient_meal_request_id",
                schema: "dbo",
                table: "trays",
                column: "patient_meal_request_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tray_ingredients",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tray_status_history",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "trays",
                schema: "dbo");
        }
    }
}
