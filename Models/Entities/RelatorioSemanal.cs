using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities
{
    public class RelatorioSemanal
    {
        public int Id { get; set; }
        public int IdMatricula { get; set; }
        public int IdLicao { get; set; }
        public string Observacoes { get; set; } = null!;
        public StatusPresenca Presenca { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataRegistro { get; set; }

        public DateTime DataLicao { get; set; }
        public Matricula Matricula { get; set; } = null!;
        public Licao Licao { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}