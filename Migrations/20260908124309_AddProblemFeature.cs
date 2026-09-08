using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemTalepTakipSistemiHalkbank.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Personeller_PersonelId",
                table: "Problems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problems",
                table: "Problems");

            migrationBuilder.RenameTable(
                name: "Problems",
                newName: "Problemler");

            migrationBuilder.RenameColumn(
                name: "İl",
                table: "Problemler",
                newName: "Sehir");

            migrationBuilder.RenameColumn(
                name: "Detay",
                table: "Problemler",
                newName: "Aciklama");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_PersonelId",
                table: "Problemler",
                newName: "IX_Problemler_PersonelId");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Personeller",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Problemler",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Durum",
                table: "Problemler",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "Problemler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Oncelik",
                table: "Problemler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problemler",
                table: "Problemler",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_IdentityUserId",
                table: "Personeller",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_AspNetUsers_IdentityUserId",
                table: "Personeller",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Problemler_Personeller_PersonelId",
                table: "Problemler",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_AspNetUsers_IdentityUserId",
                table: "Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_Problemler_Personeller_PersonelId",
                table: "Problemler");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_IdentityUserId",
                table: "Personeller");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Problemler",
                table: "Problemler");

            migrationBuilder.DropColumn(
                name: "Oncelik",
                table: "Problemler");

            migrationBuilder.RenameTable(
                name: "Problemler",
                newName: "Problems");

            migrationBuilder.RenameColumn(
                name: "Sehir",
                table: "Problems",
                newName: "İl");

            migrationBuilder.RenameColumn(
                name: "Aciklama",
                table: "Problems",
                newName: "Detay");

            migrationBuilder.RenameIndex(
                name: "IX_Problemler_PersonelId",
                table: "Problems",
                newName: "IX_Problems_PersonelId");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PersonelId",
                table: "Problems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Durum",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Problems",
                table: "Problems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Personeller_PersonelId",
                table: "Problems",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }
    }
}
