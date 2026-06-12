using EnsinoApp.ViewModels.Casal;

namespace EnsinoApp.Services.Casal;

public interface ICasalService
{
    Task<List<Models.Entities.Casal>> FindAllAsync();
    Task<Models.Entities.Casal?> FindByIdAsync(int id);
    Task<Models.Entities.Casal> CreateAsync(Models.Entities.Casal casal);
    Task UpdateAsync(Models.Entities.Casal casal);
    Task DeleteAsync(int id);

    int ContarTotal();
    List<CasalResumoViewModel> ObterResumoCasais();
    Task<IEnumerable<(string Campus, int Total)>> GetCasaisPorCampusAsync();

    Task<Models.Entities.Casal?> FindByIdComMatriculasAsync(int id);

    /// <summary>
    /// Busca um casal pelo email de qualquer um dos cônjuges.
    /// Usado para evitar duplicação ao matricular a partir de uma inscrição online.
    /// </summary>
    Task<Models.Entities.Casal?> FindByEmailAsync(string email);
}