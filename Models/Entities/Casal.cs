using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities
{
    public class Casal
    {
        public int Id { get; set; }
        public int IdCampus { get; set; }
        public int IdTurma { get; set; }
        public string NomeConjuge1 { get; set; } = null!;
        public string NomeConjuge2 { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public StatusCasal Status { get; set; }

        // Endereço
        public string Rua { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string Cep { get; set; } = null!;

        //campo opcional GC
        public string? NomeGC { get; set; }

        // Relacionamentos
        public Campus Campus { get; set; } = null!;
        public Turma Turma { get; set; } = null!;
        public ICollection<RelatorioSemanal> Relatorios { get; set; } = new List<RelatorioSemanal>();
    }
}