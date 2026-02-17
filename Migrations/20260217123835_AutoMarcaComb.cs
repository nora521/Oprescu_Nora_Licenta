using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class AutoMarcaComb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combustibil",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipCombustibil = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combustibil", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeMarca = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Autovehicul",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Poza = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarcaID = table.Column<int>(type: "int", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NrInmatriculare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CombustibilID = table.Column<int>(type: "int", nullable: true),
                    Kilometraj = table.Column<int>(type: "int", nullable: false),
                    DataITP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRCA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRovinieta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRevizie = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autovehicul", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Autovehicul_Combustibil_CombustibilID",
                        column: x => x.CombustibilID,
                        principalTable: "Combustibil",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Autovehicul_Marca_MarcaID",
                        column: x => x.MarcaID,
                        principalTable: "Marca",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autovehicul_CombustibilID",
                table: "Autovehicul",
                column: "CombustibilID");

            migrationBuilder.CreateIndex(
                name: "IX_Autovehicul_MarcaID",
                table: "Autovehicul",
                column: "MarcaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autovehicul");

            migrationBuilder.DropTable(
                name: "Combustibil");

            migrationBuilder.DropTable(
                name: "Marca");
        }
    }
}
