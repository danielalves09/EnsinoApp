namespace EnsinoApp.Models.Entities
{
    public class Licao
    {
        public int Id { get; set; }
        public int IdCurso { get; set; }
        public int NumeroSemana { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public bool Ativa { get; set; }

        public Curso Curso { get; set; } = null!;
        public ICollection<RelatorioSemanal> Relatorios { get; set; } = new List<RelatorioSemanal>();
    }
}