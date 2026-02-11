using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlteraInscricoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InscricoesOnline_Cursos_CursoId",
                table: "InscricoesOnline");

            migrationBuilder.DropIndex(
                name: "IX_InscricoesOnline_CursoId",
                table: "InscricoesOnline");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "InscricoesOnline");

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_IdCurso",
                table: "InscricoesOnline",
                column: "IdCurso");

            migrationBuilder.AddForeignKey(
                name: "FK_InscricoesOnline_Cursos_IdCurso",
                table: "InscricoesOnline",
                column: "IdCurso",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InscricoesOnline_Cursos_IdCurso",
                table: "InscricoesOnline");

            migrationBuilder.DropIndex(
                name: "IX_InscricoesOnline_IdCurso",
                table: "InscricoesOnline");

            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "InscricoesOnline",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_CursoId",
                table: "InscricoesOnline",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_InscricoesOnline_Cursos_CursoId",
                table: "InscricoesOnline",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
