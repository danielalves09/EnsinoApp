namespace EnsinoApp.Models.Entities
{
    public class TurmaLider
    {
        public int IdTurma { get; set; }
        public int IdUsuario { get; set; }

        public Turma Turma { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}