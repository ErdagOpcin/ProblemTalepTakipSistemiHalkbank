using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemTalepTakipSistemiHalkbank.Migrations
{
    /// <inheritdoc />
    public partial class AddCozulmeTarihiToProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CozulmeTarihi",
                table: "Problemler",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CozulmeTarihi",
                table: "Problemler");
        }
    }
}
