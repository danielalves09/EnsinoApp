using EnsinoApp.Repositories.LayoutCertificado;

namespace EnsinoApp.Services.LayoutCertificado;

public class LayoutCertificadoService : ILayoutCertificadoService
{
    private readonly ILayoutCertificadoRepository _repository;

    // Variáveis suportadas pelo sistema — extensível
    private static readonly List<(string Variavel, string Descricao)> _variaveis = new()
    {
        ("{{NomeCasal}}",       "Nome completo do casal (ex: João e Maria)"),
        ("{{NomeCurso}}",       "Nome do curso concluído"),
        ("{{DataConclusao}}",   "Data de conclusão do curso (dd/MM/yyyy)"),
        ("{{NomeLider}}",       "Nome dos líderes da turma"),
        ("{{NomeCampus}}",      "Nome do campus"),
        ("{{LogoUrl}}",         "URL da logomarca para uso no <img>"),
        ("{{FundoUrl}}",        "URL da imagem de fundo (use no CSS background-image)"),
        ("{{CodigoValidacao}}", "Código único de validação do certificado"),
        ("{{QRCodeBase64}}",    "QR Code em base64 para uso em <img src=\"...\">"),
    };

    public LayoutCertificadoService(ILayoutCertificadoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Models.Entities.LayoutCertificado>> FindAllAsync() =>
        _repository.FindAllAsync();

    public Task<List<Models.Entities.LayoutCertificado>> FindAllAtivosAsync() =>
        _repository.FindAllAtivosAsync();

    public Task<Models.Entities.LayoutCertificado?> FindByIdAsync(int id) =>
        _repository.FindByIdAsync(id);

    public Task<Models.Entities.LayoutCertificado> CreateAsync(Models.Entities.LayoutCertificado layout) =>
        _repository.CreateAsync(layout);

    public Task<Models.Entities.LayoutCertificado> UpdateAsync(Models.Entities.LayoutCertificado layout) =>
        _repository.UpdateAsync(layout);

    public Task DeleteAsync(int id) =>
        _repository.DeleteAsync(id);

    public string ProcessarTemplate(string templateHtml, Dictionary<string, string> variaveis)
    {
        if (string.IsNullOrWhiteSpace(templateHtml))
            return string.Empty;

        foreach (var (chave, valor) in variaveis)
        {
            // Aceita tanto {{Chave}} quanto {{ Chave }} (com espaços)
            templateHtml = templateHtml
                .Replace(chave, valor)
                .Replace($"{{{{ {chave} }}}}", valor);
        }

        return templateHtml;
    }

    public IReadOnlyList<(string Variavel, string Descricao)> GetVariaveisDisponiveis() =>
        _variaveis.AsReadOnly();
}