namespace EnsinoApp.ViewModels.Inscricao;

public class InscricaoOnlineResumoViewModel
{
    public int Id { get; set; }
    public string NomeCasal { get; set; } = string.Empty;
    public string Curso { get; set; } = string.Empty;
    public string Campus { get; set; } = string.Empty;
    public DateTime DataInscricao { get; set; }
}
