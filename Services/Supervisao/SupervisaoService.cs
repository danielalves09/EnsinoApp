
using EnsinoApp.Repositories.Supervisao;

namespace EnsinoApp.Services.Supervisao;

public class SupervisaoService : ISupervisaoService
{

    private readonly ISupervisaoRepository _supervisaoRepository;

    public SupervisaoService(ISupervisaoRepository supervisaoRepository)
    {
        _supervisaoRepository = supervisaoRepository;
    }

    public Task<int> ContarTotal()
    {
        return _supervisaoRepository.ContarTotal();
    }

    public Models.Entities.Supervisao Create(Models.Entities.Supervisao model)
    {
        var supervisaoCreated = _supervisaoRepository.Create(model);

        return supervisaoCreated;
    }

    public void Delete(int id)
    {
        _supervisaoRepository.Delete(id);
    }

    public ICollection<Models.Entities.Supervisao> FindAll()
    {
        return _supervisaoRepository.FindAll();
    }

    public ICollection<Models.Entities.Supervisao> FindAll(string filtro)
    {
        var supervisoesEncontradas = _supervisaoRepository.FindAll(filtro);

        return supervisoesEncontradas;
    }

    public Models.Entities.Supervisao? FindById(int id)
    {
        var supervisaoFind = _supervisaoRepository.FindById(id);

        return supervisaoFind;
    }

    public Models.Entities.Supervisao Update(Models.Entities.Supervisao model)
    {
        var supervisaoUpdated = _supervisaoRepository.Update(model);
        return supervisaoUpdated;
    }
}