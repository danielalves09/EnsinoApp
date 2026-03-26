
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.ViewModels.Casal;

namespace EnsinoApp.Services.Casal;

public class CasalService : ICasalService
{

    private readonly ICasalRepository _casalRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public CasalService(
        ICasalRepository casalRepository,
        IMatriculaRepository matriculaRepository)
    {
        _casalRepository = casalRepository;
        _matriculaRepository = matriculaRepository;
    }

    public Task<List<Models.Entities.Casal>> FindAllAsync() => _casalRepository.FindAllAsync();
    public Task<Models.Entities.Casal?> FindByIdAsync(int id) => _casalRepository.FindByIdAsync(id);
    public Task<Models.Entities.Casal> CreateAsync(Models.Entities.Casal casal) => _casalRepository.CreateAsync(casal);
    public Task UpdateAsync(Models.Entities.Casal casal) => _casalRepository.UpdateAsync(casal);
    public Task DeleteAsync(int id) => _casalRepository.DeleteAsync(id);



    public int ContarTotal()
    {
        return _casalRepository.ContarTotal();
    }

    public List<CasalResumoViewModel> ObterResumoCasais()
    {
        return _casalRepository.ObterResumoCasais();
    }

    public async Task<IEnumerable<(string Campus, int Total)>> GetCasaisPorCampusAsync()
    {
        return await _casalRepository.GetCasaisPorCampusAsync();
    }
}

