using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenamePatientMealRequestToPatientRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "patient_meal_requests",
                schema: "dbo",
                newName: "patient_requests");

            migrationBuilder.RenameIndex(
                name: "IX_patient_meal_requests_recipe_id",
                schema: "dbo",
                table: "patient_requests",
                newName: "IX_patient_requests_recipe_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_patient_requests_recipe_id",
                schema: "dbo",
                table: "patient_requests",
                newName: "IX_patient_meal_requests_recipe_id");

            migrationBuilder.RenameTable(
                name: "patient_requests",
                schema: "dbo",
                newName: "patient_meal_requests");
        }
    }
}
