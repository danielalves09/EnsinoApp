namespace EnsinoApp.ViewModels.LayoutCertificado;

public class ListarLayoutCertificadoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }
    public string Orientacao { get; set; } = null!;
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int QtdCursosVinculados { get; set; }
}