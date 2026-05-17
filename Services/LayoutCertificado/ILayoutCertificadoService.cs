using EnsinoApp.Models.Entities;

namespace EnsinoApp.Services.LayoutCertificado;

public interface ILayoutCertificadoService
{
    Task<List<Models.Entities.LayoutCertificado>> FindAllAsync();
    Task<List<Models.Entities.LayoutCertificado>> FindAllAtivosAsync();
    Task<Models.Entities.LayoutCertificado?> FindByIdAsync(int id);
    Task<Models.Entities.LayoutCertificado> CreateAsync(Models.Entities.LayoutCertificado layout);
    Task<Models.Entities.LayoutCertificado> UpdateAsync(Models.Entities.LayoutCertificado layout);
    Task DeleteAsync(int id);

    /// <summary>
    /// Processa o template HTML substituindo as variáveis {{Variavel}} pelos valores reais.
    /// </summary>
    string ProcessarTemplate(string templateHtml, Dictionary<string, string> variaveis);

    /// <summary>Retorna as variáveis disponíveis com descrição para exibição no editor.</summary>
    IReadOnlyList<(string Variavel, string Descricao)> GetVariaveisDisponiveis();
}