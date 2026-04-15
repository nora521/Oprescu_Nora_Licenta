using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class Rezervari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CNP",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "SeriePermis",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rezervare",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: true),
                    AutovehiculID = table.Column<int>(type: "int", nullable: true),
                    DataStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PretZi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PretTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervare", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rezervare_Autovehicul_AutovehiculID",
                        column: x => x.AutovehiculID,
                        principalTable: "Autovehicul",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Rezervare_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervare_AutovehiculID",
                table: "Rezervare",
                column: "AutovehiculID");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervare_UtilizatorID",
                table: "Rezervare",
                column: "UtilizatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rezervare");

            migrationBuilder.DropColumn(
                name: "SeriePermis",
                table: "Utilizator");

            migrationBuilder.AlterColumn<string>(
                name: "CNP",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
