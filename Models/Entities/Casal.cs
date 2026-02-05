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
        public string Rua { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string Cep { get; set; } = null!;


        // Relacionamentos
        public Campus Campus { get; set; } = null!;
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}