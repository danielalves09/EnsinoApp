namespace EnsinoApp.ViewModels.Lider;

public class LiderDashboardViewModel
{
    public string NomeLider { get; set; } = string.Empty;

    // Resumo de Turmas
    public int TotalTurmasAtivas { get; set; }
    public int TotalTurmasConcluidas { get; set; }
    public int TotalCasaisAtivos { get; set; }

    // Relatórios
    public int TotalRelatoriosLancados { get; set; }
    public int TotalRelatoriosPendentes { get; set; }

    // Listas detalhadas
    public List<TurmaResumoViewModel> Turmas { get; set; } = new List<TurmaResumoViewModel>();
}