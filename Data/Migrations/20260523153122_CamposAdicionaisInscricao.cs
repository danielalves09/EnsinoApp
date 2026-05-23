using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CamposAdicionaisInscricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Convidado",
                table: "InscricoesOnline",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimentoEsposa",
                table: "InscricoesOnline",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimentoMarido",
                table: "InscricoesOnline",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeCasalConvidador",
                table: "InscricoesOnline",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Convidado",
                table: "InscricoesOnline");

            migrationBuilder.DropColumn(
                name: "DataNascimentoEsposa",
                table: "InscricoesOnline");

            migrationBuilder.DropColumn(
                name: "DataNascimentoMarido",
                table: "InscricoesOnline");

            migrationBuilder.DropColumn(
                name: "NomeCasalConvidador",
                table: "InscricoesOnline");
        }
    }
}
