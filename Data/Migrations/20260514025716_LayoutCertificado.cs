using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class LayoutCertificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdLayoutCertificado",
                table: "Cursos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LayoutsCertificado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TemplateHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagemFundoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Orientacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Landscape"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutsCertificado", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_IdLayoutCertificado",
                table: "Cursos",
                column: "IdLayoutCertificado");

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_LayoutsCertificado_IdLayoutCertificado",
                table: "Cursos",
                column: "IdLayoutCertificado",
                principalTable: "LayoutsCertificado",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_LayoutsCertificado_IdLayoutCertificado",
                table: "Cursos");

            migrationBuilder.DropTable(
                name: "LayoutsCertificado");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_IdLayoutCertificado",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "IdLayoutCertificado",
                table: "Cursos");
        }
    }
}
