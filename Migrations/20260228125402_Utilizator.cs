using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class Utilizator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilizatorID",
                table: "Autovehicul",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Utilizator",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CNP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NrTelefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parola = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizator", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autovehicul_UtilizatorID",
                table: "Autovehicul",
                column: "UtilizatorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Autovehicul_Utilizator_UtilizatorID",
                table: "Autovehicul",
                column: "UtilizatorID",
                principalTable: "Utilizator",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Autovehicul_Utilizator_UtilizatorID",
                table: "Autovehicul");

            migrationBuilder.DropTable(
                name: "Utilizator");

            migrationBuilder.DropIndex(
                name: "IX_Autovehicul_UtilizatorID",
                table: "Autovehicul");

            migrationBuilder.DropColumn(
                name: "UtilizatorID",
                table: "Autovehicul");
        }
    }
}
