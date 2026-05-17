namespace EnsinoApp.ViewModels.LayoutCertificado;

public class PreviewLayoutViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public string Orientacao { get; set; } = null!;
    public string HtmlProcessado { get; set; } = null!;
    public List<(string Variavel, string Descricao)> VariaveisDisponiveis { get; set; } = new();
}