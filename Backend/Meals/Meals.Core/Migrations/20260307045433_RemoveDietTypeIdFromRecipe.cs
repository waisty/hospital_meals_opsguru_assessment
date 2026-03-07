using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDietTypeIdFromRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipes_diet_types_diet_type_id",
                schema: "dbo",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "IX_recipes_diet_type_id",
                schema: "dbo",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "diet_type_id",
                schema: "dbo",
                table: "recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "diet_type_id",
                schema: "dbo",
                table: "recipes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipes_diet_type_id",
                schema: "dbo",
                table: "recipes",
                column: "diet_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_recipes_diet_types_diet_type_id",
                schema: "dbo",
                table: "recipes",
                column: "diet_type_id",
                principalSchema: "dbo",
                principalTable: "diet_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
