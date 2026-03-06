using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class UniqueNameIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_recipes_name",
                schema: "dbo",
                table: "recipes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meals_name",
                schema: "dbo",
                table: "meals",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_name",
                schema: "dbo",
                table: "ingredients",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_diet_types_name",
                schema: "dbo",
                table: "diet_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clinical_states_name",
                schema: "dbo",
                table: "clinical_states",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_allergies_name",
                schema: "dbo",
                table: "allergies",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_recipes_name",
                schema: "dbo",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "IX_meals_name",
                schema: "dbo",
                table: "meals");

            migrationBuilder.DropIndex(
                name: "IX_ingredients_name",
                schema: "dbo",
                table: "ingredients");

            migrationBuilder.DropIndex(
                name: "IX_diet_types_name",
                schema: "dbo",
                table: "diet_types");

            migrationBuilder.DropIndex(
                name: "IX_clinical_states_name",
                schema: "dbo",
                table: "clinical_states");

            migrationBuilder.DropIndex(
                name: "IX_allergies_name",
                schema: "dbo",
                table: "allergies");
        }
    }
}
