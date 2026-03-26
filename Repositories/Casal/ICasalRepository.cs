using EnsinoApp.ViewModels.Casal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EnsinoApp.Repositories.Casal;


public interface ICasalRepository
{

    Task<List<Models.Entities.Casal>> FindAllAsync();
    Task<Models.Entities.Casal?> FindByIdAsync(int id);
    Task<Models.Entities.Casal> CreateAsync(Models.Entities.Casal casal);
    Task UpdateAsync(Models.Entities.Casal casal);
    Task DeleteAsync(int id);
    int ContarTotal();
    List<Models.Entities.Casal> ObterTodos();
    Task<IEnumerable<(string Campus, int Total)>> GetCasaisPorCampusAsync();

    public List<CasalResumoViewModel> ObterResumoCasais();
}