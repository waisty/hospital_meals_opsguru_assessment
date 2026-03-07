using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Meals.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameRequestedForDateToRequestedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requested_for_date",
                schema: "dbo",
                table: "patient_requests",
                newName: "requested_date_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requested_date_time",
                schema: "dbo",
                table: "patient_requests",
                newName: "requested_for_date");
        }
    }
}
