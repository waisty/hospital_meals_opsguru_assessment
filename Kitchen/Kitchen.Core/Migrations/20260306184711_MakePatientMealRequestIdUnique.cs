using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Kitchen.Core.Migrations
{
    /// <inheritdoc />
    public partial class MakePatientMealRequestIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trays_patient_meal_request_id",
                schema: "dbo",
                table: "trays");

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
            migrationBuilder.DropIndex(
                name: "IX_trays_patient_meal_request_id",
                schema: "dbo",
                table: "trays");

            migrationBuilder.CreateIndex(
                name: "IX_trays_patient_meal_request_id",
                schema: "dbo",
                table: "trays",
                column: "patient_meal_request_id");
        }
    }
}
