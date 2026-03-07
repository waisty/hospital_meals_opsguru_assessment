using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Kitchen.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTrayReceivedAndLastUpdateDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_update_date_time",
                schema: "dbo",
                table: "trays",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date_time",
                schema: "dbo",
                table: "trays",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_update_date_time",
                schema: "dbo",
                table: "trays");

            migrationBuilder.DropColumn(
                name: "received_date_time",
                schema: "dbo",
                table: "trays");
        }
    }
}
