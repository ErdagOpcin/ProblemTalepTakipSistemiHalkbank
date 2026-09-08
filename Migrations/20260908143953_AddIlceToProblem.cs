using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemTalepTakipSistemiHalkbank.Migrations
{
    /// <inheritdoc />
    public partial class AddIlceToProblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "Problemler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "Problemler");
        }
    }
}
