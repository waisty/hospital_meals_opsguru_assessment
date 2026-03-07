using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Auth.Core.Migrations
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
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    admin = table.Column<bool>(type: "boolean", nullable: false),
                    patient_admin = table.Column<bool>(type: "boolean", nullable: false),
                    meals_admin = table.Column<bool>(type: "boolean", nullable: false),
                    meals_user = table.Column<bool>(type: "boolean", nullable: false),
                    kitchen_user = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.username);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_name",
                schema: "dbo",
                table: "users",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "dbo");
        }
    }
}
