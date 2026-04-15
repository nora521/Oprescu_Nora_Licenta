using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class AddAutovehiculFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrBagaje",
                table: "Autovehicul",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NrLocuri",
                table: "Autovehicul",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransmisieID",
                table: "Autovehicul",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transmisie",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipTransmisie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transmisie", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autovehicul_TransmisieID",
                table: "Autovehicul",
                column: "TransmisieID");

            migrationBuilder.AddForeignKey(
                name: "FK_Autovehicul_Transmisie_TransmisieID",
                table: "Autovehicul",
                column: "TransmisieID",
                principalTable: "Transmisie",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Autovehicul_Transmisie_TransmisieID",
                table: "Autovehicul");

            migrationBuilder.DropTable(
                name: "Transmisie");

            migrationBuilder.DropIndex(
                name: "IX_Autovehicul_TransmisieID",
                table: "Autovehicul");

            migrationBuilder.DropColumn(
                name: "NrBagaje",
                table: "Autovehicul");

            migrationBuilder.DropColumn(
                name: "NrLocuri",
                table: "Autovehicul");

            migrationBuilder.DropColumn(
                name: "TransmisieID",
                table: "Autovehicul");
        }
    }
}
