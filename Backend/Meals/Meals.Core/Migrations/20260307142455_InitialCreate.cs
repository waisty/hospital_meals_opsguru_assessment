using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Hospital.Meals.Core.Migrations
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
                name: "allergies",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allergies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clinical_states",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinical_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "diet_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diet_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_allergy_exclusions",
                schema: "dbo",
                columns: table => new
                {
                    ingredient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    allergy_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_allergy_exclusions", x => new { x.ingredient_id, x.allergy_id });
                    table.ForeignKey(
                        name: "FK_ingredient_allergy_exclusions_allergies_allergy_id",
                        column: x => x.allergy_id,
                        principalSchema: "dbo",
                        principalTable: "allergies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ingredient_allergy_exclusions_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalSchema: "dbo",
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ingredient_clinical_state_exclusions",
                schema: "dbo",
                columns: table => new
                {
                    ingredient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    clinical_state_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_clinical_state_exclusions", x => new { x.ingredient_id, x.clinical_state_id });
                    table.ForeignKey(
                        name: "FK_ingredient_clinical_state_exclusions_clinical_states_clinic~",
                        column: x => x.clinical_state_id,
                        principalSchema: "dbo",
                        principalTable: "clinical_states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ingredient_clinical_state_exclusions_ingredients_ingredient~",
                        column: x => x.ingredient_id,
                        principalSchema: "dbo",
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "meals",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    recipe_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    diet_type_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meals", x => x.id);
                    table.ForeignKey(
                        name: "FK_meals_diet_types_diet_type_id",
                        column: x => x.diet_type_id,
                        principalSchema: "dbo",
                        principalTable: "diet_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_meals_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "dbo",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_requests",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    patient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    patient_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    first_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    last_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    recipe_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    requested_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approval_status = table.Column<int>(type: "integer", nullable: false),
                    status_reason = table.Column<string>(type: "text", nullable: true),
                    unsafe_ingredient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    finalized_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    search_vector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "first_name", "last_name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_patient_requests_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "dbo",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                schema: "dbo",
                columns: table => new
                {
                    recipe_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ingredient_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredients", x => new { x.recipe_id, x.ingredient_id });
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalSchema: "dbo",
                        principalTable: "ingredients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "dbo",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allergies_name",
                schema: "dbo",
                table: "allergies",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clinical_states_name",
                schema: "dbo",
                table: "clinical_states",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_diet_types_name",
                schema: "dbo",
                table: "diet_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_allergy_exclusions_allergy_id",
                schema: "dbo",
                table: "ingredient_allergy_exclusions",
                column: "allergy_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_clinical_state_exclusions_clinical_state_id",
                schema: "dbo",
                table: "ingredient_clinical_state_exclusions",
                column: "clinical_state_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_diet_type_exclusions_diet_type_id",
                schema: "dbo",
                table: "ingredient_diet_type_exclusions",
                column: "diet_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_name",
                schema: "dbo",
                table: "ingredients",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meals_diet_type_id",
                schema: "dbo",
                table: "meals",
                column: "diet_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_meals_name",
                schema: "dbo",
                table: "meals",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meals_recipe_id",
                schema: "dbo",
                table: "meals",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_requests_recipe_id",
                schema: "dbo",
                table: "patient_requests",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_requests_search_vector",
                schema: "dbo",
                table: "patient_requests",
                column: "search_vector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_ingredient_id",
                schema: "dbo",
                table: "recipe_ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_name",
                schema: "dbo",
                table: "recipes",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ingredient_allergy_exclusions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingredient_clinical_state_exclusions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingredient_diet_type_exclusions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "meals",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "patient_requests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "recipe_ingredients",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "allergies",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "clinical_states",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "diet_types",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingredients",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "recipes",
                schema: "dbo");
        }
    }
}
