namespace EnsinoApp.ViewModels.Inscricao;

public class InscricaoOnlineResumoViewModel
{
    public int Id { get; set; }
    public string NomeCasal { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;
    public int IdCurso { get; set; }
    public string Campus { get; set; } = string.Empty;
    public int IdCampus { get; set; }
    public DateTime DataInscricao { get; set; }
}
