using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "search_vector",
                schema: "dbo",
                table: "recipes",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            // Backfill existing rows: generated column can only be set to DEFAULT
            migrationBuilder.Sql(
                "UPDATE dbo.recipes SET search_vector = DEFAULT WHERE search_vector IS NULL");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector",
                schema: "dbo",
                table: "recipes",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector",
                oldNullable: true)
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.CreateIndex(
                name: "IX_recipes_search_vector",
                schema: "dbo",
                table: "recipes",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_recipes_search_vector",
                schema: "dbo",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "search_vector",
                schema: "dbo",
                table: "recipes");
        }
    }
}
