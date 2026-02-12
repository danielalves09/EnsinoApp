namespace EnsinoApp.ViewModels.Relatorios;

public class RelatorioCreateViewModel
{
    public int IdTurma { get; set; }
    public int IdLicao { get; set; }
    public List<CasalPresencaItem> Casais { get; set; } = new();
    public string Observacoes { get; set; } = null!;
}