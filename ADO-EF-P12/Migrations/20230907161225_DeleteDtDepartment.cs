using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADO_EF_P12.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDtDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDt",
                table: "Departments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDt",
                table: "Departments");
        }
    }
}
