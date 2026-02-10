namespace EnsinoApp.ViewModels.Matricula;

public class MatriculaViewModel
{
    public int Id { get; set; }
    public int IdCasal { get; set; }
    public string NomeCasal { get; set; } = null!;
    public int IdTurma { get; set; }
    public string NomeTurma { get; set; } = null!;
    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? NomeGC { get; set; }
    public string Status { get; set; } = null!;
}
