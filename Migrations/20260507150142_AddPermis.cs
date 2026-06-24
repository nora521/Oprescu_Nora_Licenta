using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Licenta.Migrations
{
    /// <inheritdoc />
    public partial class AddPermis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CNP",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoriiPermis",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEmiterePermis",
                table: "Utilizator",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataExpirarePermis",
                table: "Utilizator",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNasterii",
                table: "Utilizator",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumarPermis",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermisFataPath",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermisVerificat",
                table: "Utilizator",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PermisVersoPath",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriiPermis",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "DataEmiterePermis",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "DataExpirarePermis",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "DataNasterii",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "NumarPermis",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "PermisFataPath",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "PermisVerificat",
                table: "Utilizator");

            migrationBuilder.DropColumn(
                name: "PermisVersoPath",
                table: "Utilizator");

            migrationBuilder.AlterColumn<string>(
                name: "CNP",
                table: "Utilizator",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
