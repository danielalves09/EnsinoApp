using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class diaSemana : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiasDisponiveis",
                table: "PeriodosInscricao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaPreferencial",
                table: "InscricoesOnline",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasDisponiveis",
                table: "PeriodosInscricao");

            migrationBuilder.DropColumn(
                name: "DiaPreferencial",
                table: "InscricoesOnline");
        }
    }
}
