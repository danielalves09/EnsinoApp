namespace EnsinoApp.Models.Entities
{
    public class Curso
    {
        public int Id { get; set; }
        public int IdCampus { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public bool Ativo { get; set; }

        public Campus Campus { get; set; } = null!;
        public ICollection<Licao> Licoes { get; set; } = new List<Licao>();
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
    }
}