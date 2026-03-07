using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDietTypeFromMeals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meals_diet_types_diet_type_id",
                schema: "dbo",
                table: "meals");

            migrationBuilder.DropIndex(
                name: "IX_meals_diet_type_id",
                schema: "dbo",
                table: "meals");

            migrationBuilder.DropColumn(
                name: "diet_type_id",
                schema: "dbo",
                table: "meals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "diet_type_id",
                schema: "dbo",
                table: "meals",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_meals_diet_type_id",
                schema: "dbo",
                table: "meals",
                column: "diet_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_meals_diet_types_diet_type_id",
                schema: "dbo",
                table: "meals",
                column: "diet_type_id",
                principalSchema: "dbo",
                principalTable: "diet_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
