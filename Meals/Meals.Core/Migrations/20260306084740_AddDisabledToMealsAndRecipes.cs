using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDisabledToMealsAndRecipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "disabled",
                schema: "dbo",
                table: "recipes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "disabled",
                schema: "dbo",
                table: "meals",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "disabled",
                schema: "dbo",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "disabled",
                schema: "dbo",
                table: "meals");
        }
    }
}
