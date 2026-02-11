using EnsinoApp.Repositories.Licao;

namespace EnsinoApp.Services.Licao;

public class LicaoService : ILicaoService
{

    private readonly ILicaoRepository _repository;

    public LicaoService(ILicaoRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Models.Entities.Licao>> FindByCursoAsync(int cursoId) =>
        _repository.FindByCursoAsync(cursoId);

    public Task<Models.Entities.Licao> FindByIdAsync(int id) =>
        _repository.FindByIdAsync(id);

    public Task<Models.Entities.Licao> CreateAsync(Models.Entities.Licao licao) =>
        _repository.CreateAsync(licao);

    public Task<Models.Entities.Licao> UpdateAsync(Models.Entities.Licao licao) =>
        _repository.UpdateAsync(licao);

    public Task DeleteAsync(int id) =>
        _repository.DeleteAsync(id);


}