using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class CatAutoID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategorieID",
                table: "Autovehicul",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autovehicul_CategorieID",
                table: "Autovehicul",
                column: "CategorieID");

            migrationBuilder.AddForeignKey(
                name: "FK_Autovehicul_Categorie_CategorieID",
                table: "Autovehicul",
                column: "CategorieID",
                principalTable: "Categorie",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Autovehicul_Categorie_CategorieID",
                table: "Autovehicul");

            migrationBuilder.DropIndex(
                name: "IX_Autovehicul_CategorieID",
                table: "Autovehicul");

            migrationBuilder.DropColumn(
                name: "CategorieID",
                table: "Autovehicul");
        }
    }
}
