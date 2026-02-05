using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities
{
    public class Turma
    {
        public int Id { get; set; }
        public int IdCurso { get; set; }
        public int IdCampus { get; set; }
        public int IdLider { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public StatusTurma Status { get; set; } = StatusTurma.Acomecar;

        public Curso Curso { get; set; } = null!;
        public Campus Campus { get; set; } = null!;
        public Usuario Lider { get; set; } = null!;
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}