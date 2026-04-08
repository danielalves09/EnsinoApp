using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PeriodoInscricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeriodosInscricao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    IdCampus = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VagasTotal = table.Column<int>(type: "int", nullable: false),
                    VagasOcupadas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosInscricao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodosInscricao_Campuses_IdCampus",
                        column: x => x.IdCampus,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PeriodosInscricao_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosInscricao_IdCampus",
                table: "PeriodosInscricao",
                column: "IdCampus");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosInscricao_IdCurso",
                table: "PeriodosInscricao",
                column: "IdCurso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodosInscricao");
        }
    }
}
