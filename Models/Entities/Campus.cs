namespace EnsinoApp.Models.Entities
{
    public class Campus
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Telefone { get; set; } = null!;

        // Endereço
        public string Rua { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string Cep { get; set; } = null!;

        // Relacionamentos
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Casal> Casais { get; set; } = new List<Casal>();
        public ICollection<Supervisao> Supervisoes { get; set; } = new List<Supervisao>();
    }
}