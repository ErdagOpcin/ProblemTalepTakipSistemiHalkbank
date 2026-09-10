using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemTalepTakipSistemiHalkbank.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemPersonelManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problemler_Personeller_PersonelId",
                table: "Problemler");

            migrationBuilder.DropIndex(
                name: "IX_Problemler_PersonelId",
                table: "Problemler");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "Problemler");

            migrationBuilder.CreateTable(
                name: "ProblemPersoneller",
                columns: table => new
                {
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemPersoneller", x => new { x.ProblemId, x.PersonelId });
                    table.ForeignKey(
                        name: "FK_ProblemPersoneller_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemPersoneller_Problemler_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemPersoneller_PersonelId",
                table: "ProblemPersoneller",
                column: "PersonelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemPersoneller");

            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "Problemler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Problemler_PersonelId",
                table: "Problemler",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problemler_Personeller_PersonelId",
                table: "Problemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
