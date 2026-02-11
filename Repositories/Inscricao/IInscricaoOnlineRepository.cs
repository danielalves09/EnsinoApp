using EnsinoApp.ViewModels.Inscricao;

namespace EnsinoApp.Repositories.Inscricao;

public interface IInscricaoOnlineRepository
{
    Task<List<Models.Entities.InscricaoOnline>> FindAllAsync();
    Task<Models.Entities.InscricaoOnline?> FindByIdAsync(int id);
    Task<Models.Entities.InscricaoOnline?> CreateAsync(Models.Entities.InscricaoOnline inscricao);
    Task UpdateAsync(Models.Entities.InscricaoOnline inscricao);
    Task DeleteAsync(int id);

    int ContarTotal();
    int ContarPendentes();
    List<InscricaoOnlineResumoViewModel> ObterPendentesResumo();
}
