using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsinoApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class retiraUnicidadeEmailInscricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InscricoesOnline_EmailEsposa",
                table: "InscricoesOnline");

            migrationBuilder.DropIndex(
                name: "IX_InscricoesOnline_EmailMarido",
                table: "InscricoesOnline");

            migrationBuilder.AlterColumn<string>(
                name: "EmailMarido",
                table: "InscricoesOnline",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmailEsposa",
                table: "InscricoesOnline",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailMarido",
                table: "InscricoesOnline",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmailEsposa",
                table: "InscricoesOnline",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
        }
    }
}
