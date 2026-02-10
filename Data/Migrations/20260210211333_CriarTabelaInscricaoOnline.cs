using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaInscricaoOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InscricoesOnline",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeMarido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeEsposa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefoneMarido = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TelefoneEsposa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmailMarido = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailEsposa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCampus = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    ParticipaGC = table.Column<bool>(type: "bit", nullable: false),
                    NomeGC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Processada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscricoesOnline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscricoesOnline_Campuses_IdCampus",
                        column: x => x.IdCampus,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InscricoesOnline_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_CursoId",
                table: "InscricoesOnline",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_EmailEsposa",
                table: "InscricoesOnline",
                column: "EmailEsposa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_EmailMarido",
                table: "InscricoesOnline",
                column: "EmailMarido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesOnline_IdCampus",
                table: "InscricoesOnline",
                column: "IdCampus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscricoesOnline");
        }
    }
}
