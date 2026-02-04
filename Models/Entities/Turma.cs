using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities
{
    public class Turma
    {
        public int Id { get; set; }
        public int IdCurso { get; set; }
        public int IdCampus { get; set; }
        public int IdCoordenador { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public StatusTurma Status { get; set; }

        public Curso Curso { get; set; } = null!;
        public Campus Campus { get; set; } = null!;
        public Usuario Coordenador { get; set; } = null!;
        public ICollection<TurmaLider> Lideres { get; set; } = new List<TurmaLider>();
        public ICollection<Casal> Casais { get; set; } = new List<Casal>();
        public ICollection<RelatorioSemanal> Relatorios { get; set; } = new List<RelatorioSemanal>();
    }
}