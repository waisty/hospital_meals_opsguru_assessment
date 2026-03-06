using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientDietTypeExclusions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingredient_diet_type_exclusions",
                schema: "dbo",
                columns: table => new
                {
                    ingredient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    diet_type_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_diet_type_exclusions", x => new { x.ingredient_id, x.diet_type_id });
                    table.ForeignKey(
                        name: "FK_ingredient_diet_type_exclusions_diet_types_diet_type_id",
                        column: x => x.diet_type_id,
                        principalSchema: "dbo",
                        principalTable: "diet_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ingredient_diet_type_exclusions_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalSchema: "dbo",
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_diet_type_exclusions_diet_type_id",
                schema: "dbo",
                table: "ingredient_diet_type_exclusions",
                column: "diet_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingredient_diet_type_exclusions",
                schema: "dbo");
        }
    }
}
