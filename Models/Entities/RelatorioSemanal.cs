namespace EnsinoApp.Models.Entities
{
    public class RelatorioSemanal
    {
        public int Id { get; set; }
        public int IdCasal { get; set; }
        public int IdTurma { get; set; }
        public int IdLicao { get; set; }
        public string Observacoes { get; set; } = null!;
        public int? Avaliacao { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataRegistro { get; set; }

        public DateTime DataLicao { get; set; }

        public Casal Casal { get; set; } = null!;
        public Turma Turma { get; set; } = null!;
        public Licao Licao { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}