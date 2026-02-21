using EnsinoApp.Models.Enums;

namespace EnsinoApp.Models.Entities
{
    public class Casal
    {
        public int Id { get; set; }
        public int IdCampus { get; set; }
        public string NomeConjuge1 { get; set; } = null!;
        public string NomeConjuge2 { get; set; } = null!;
        public string TelefoneConjuge1 { get; set; } = null!;
        public string TelefoneConjuge2 { get; set; } = null!;
        public string EmailConjuge1 { get; set; } = null!;
        public string EmailConjuge2 { get; set; } = null!;

        public StatusCasal Status { get; set; }

        // Endereço
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;


        // Relacionamentos
        public Campus Campus { get; set; } = null!;
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}