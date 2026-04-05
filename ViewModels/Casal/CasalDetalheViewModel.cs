namespace EnsinoApp.ViewModels.Casal;

public class CasalDetalheViewModel
{
    public int Id { get; set; }

    // Cônjuges
    public string NomeConjuge1 { get; set; } = string.Empty;
    public string NomeConjuge2 { get; set; } = string.Empty;
    public string EmailConjuge1 { get; set; } = string.Empty;
    public string EmailConjuge2 { get; set; } = string.Empty;
    public string TelefoneConjuge1 { get; set; } = string.Empty;
    public string TelefoneConjuge2 { get; set; } = string.Empty;

    // Endereço
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;

    // Vínculo
    public string NomeCampus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Histórico de matrículas
    public List<MatriculaResumoViewModel> Matriculas { get; set; } = new();

    public class MatriculaResumoViewModel
    {
        public string NomeCurso { get; set; } = string.Empty;
        public string NomeTurma { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DataMatricula { get; set; }
        public DateTime? DataConclusao { get; set; }
        public bool CertificadoEmitido { get; set; }
    }
}