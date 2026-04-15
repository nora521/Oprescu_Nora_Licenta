using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class AutoCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "AutoCategorie",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategorieID = table.Column<int>(type: "int", nullable: false),
                    AutovehiculID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoCategorie", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AutoCategorie_Autovehicul_AutovehiculID",
                        column: x => x.AutovehiculID,
                        principalTable: "Autovehicul",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutoCategorie_Categorie_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categorie",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoCategorie_AutovehiculID",
                table: "AutoCategorie",
                column: "AutovehiculID");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCategorie_CategorieID",
                table: "AutoCategorie",
                column: "CategorieID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoCategorie");

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
    }
}
