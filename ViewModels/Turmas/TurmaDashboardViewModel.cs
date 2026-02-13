using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Turmas
{
    public class TurmaDashboardViewModel
    {
        public int Id { get; set; }
        public string NomeCurso { get; set; } = null!;
        public string NomeCampus { get; set; } = null!;
        public string NomeLider { get; set; } = null!;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public StatusTurma Status { get; set; }

        public List<CasalMatriculadoViewModel> CasaisMatriculados { get; set; } = new List<CasalMatriculadoViewModel>();


        public int TotalCasais => CasaisMatriculados.Count;
        public int CasaisPresentes => CasaisMatriculados.Count(c => c.Presenca == StatusPresenca.Presente);
        public int CasaisAusentes => CasaisMatriculados.Count(c => c.Presenca == StatusPresenca.Ausente);
        public decimal PercentualPresenca => CasaisMatriculados.Any() ? (decimal)CasaisPresentes / CasaisMatriculados.Count * 100 : 0;

        public int TotalLicoes { get; set; }
        public int LicoesConcluidas { get; set; }

        public int Progresso => TotalLicoes == 0 ? 0 : (int)((LicoesConcluidas / (double)TotalLicoes) * 100);
    }
}
