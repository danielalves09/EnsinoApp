namespace EnsinoApp.Models.Entities
{
    public class Campus
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;

        // Endereço
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }

        // Relacionamentos
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Casal> Casais { get; set; } = new List<Casal>();
        public ICollection<Supervisao> Supervisoes { get; set; } = new List<Supervisao>();
    }
}