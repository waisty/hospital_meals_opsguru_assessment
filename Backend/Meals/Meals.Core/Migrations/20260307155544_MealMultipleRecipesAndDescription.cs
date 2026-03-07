using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class MealMultipleRecipesAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add description column to meals
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "dbo",
                table: "meals",
                type: "text",
                nullable: true);

            // 2. Create meal_recipes table
            migrationBuilder.CreateTable(
                name: "meal_recipes",
                schema: "dbo",
                columns: table => new
                {
                    meal_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    recipe_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_recipes", x => new { x.meal_id, x.recipe_id });
                    table.ForeignKey(
                        name: "FK_meal_recipes_meals_meal_id",
                        column: x => x.meal_id,
                        principalSchema: "dbo",
                        principalTable: "meals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_meal_recipes_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "dbo",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meal_recipes_recipe_id",
                schema: "dbo",
                table: "meal_recipes",
                column: "recipe_id");

            // 3. Migrate existing meal-recipe links from meals.recipe_id to meal_recipes
            migrationBuilder.Sql(
                "INSERT INTO dbo.meal_recipes (meal_id, recipe_id, disabled) SELECT id, recipe_id, false FROM dbo.meals WHERE recipe_id IS NOT NULL AND recipe_id != ''");

            // 4. Drop FK, index and recipe_id from meals
            migrationBuilder.DropForeignKey(
                name: "FK_meals_recipes_recipe_id",
                schema: "dbo",
                table: "meals");

            migrationBuilder.DropIndex(
                name: "IX_meals_recipe_id",
                schema: "dbo",
                table: "meals");

            migrationBuilder.DropColumn(
                name: "recipe_id",
                schema: "dbo",
                table: "meals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meal_recipes",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "dbo",
                table: "meals");

            migrationBuilder.AddColumn<string>(
                name: "recipe_id",
                schema: "dbo",
                table: "meals",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_meals_recipe_id",
                schema: "dbo",
                table: "meals",
                column: "recipe_id");

            migrationBuilder.AddForeignKey(
                name: "FK_meals_recipes_recipe_id",
                schema: "dbo",
                table: "meals",
                column: "recipe_id",
                principalSchema: "dbo",
                principalTable: "recipes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
