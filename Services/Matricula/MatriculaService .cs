using EnsinoApp.Repositories.Matricula;

namespace EnsinoApp.Services.Matricula;


public class MatriculaService : IMatriculaService
{
    private readonly IMatriculaRepository _repository;

    public MatriculaService(IMatriculaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Models.Entities.Matricula>> FindAllAsync() => _repository.FindAllAsync();
    public Task<Models.Entities.Matricula?> FindByIdAsync(int id) => _repository.FindByIdAsync(id);
    public Task CreateAsync(Models.Entities.Matricula matricula) => _repository.CreateAsync(matricula);
    public Task UpdateAsync(Models.Entities.Matricula matricula) => _repository.UpdateAsync(matricula);
    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
}