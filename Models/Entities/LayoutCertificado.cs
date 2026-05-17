namespace EnsinoApp.Models.Entities;

public class LayoutCertificado
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descricao { get; set; }


    public string TemplateHtml { get; set; } = null!;


    public string? ImagemFundoUrl { get; set; }

    public string Orientacao { get; set; } = "Landscape";

    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Relacionamento
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}