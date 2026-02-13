using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Lider;

public class TurmaResumoViewModel
{
    public int Id { get; set; }
    public string NomeCurso { get; set; } = string.Empty;
    public string NomeCampus { get; set; } = string.Empty;
    public int TotalCasais { get; set; }
    public int TotalRelatoriosLancados { get; set; }
    public int TotalRelatoriosPendentes { get; set; }
    public StatusTurma StatusTurma { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }

    public int TotalLicoes { get; set; }
    public int LicoesConcluidas { get; set; }

    public int Progresso => TotalLicoes == 0 ? 0 : (int)((LicoesConcluidas / (double)TotalLicoes) * 100);
}