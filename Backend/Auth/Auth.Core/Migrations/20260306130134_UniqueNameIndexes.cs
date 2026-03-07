using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Auth.Core.Migrations
{
    /// <inheritdoc />
    public partial class UniqueNameIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
            migrationBuilder.DropIndex(
                name: "IX_users_name",
                schema: "dbo",
                table: "users");
        }
    }
}
