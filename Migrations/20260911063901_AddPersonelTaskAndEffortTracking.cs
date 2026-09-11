using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemTalepTakipSistemiHalkbank.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonelTaskAndEffortTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonelTasklari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanlananEfor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HarcananEfor = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelTasklari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelTasklari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonelTasklari_PersonelId",
                table: "PersonelTasklari",
                column: "PersonelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonelTasklari");
        }
    }
}
