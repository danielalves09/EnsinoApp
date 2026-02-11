namespace EnsinoApp.ViewModels.Turmas;

public class TurmaResumoViewModel
{
    public int Id { get; set; }
    public string Curso { get; set; } = string.Empty;
    public string Campus { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int TotalMatriculados { get; set; }
}
