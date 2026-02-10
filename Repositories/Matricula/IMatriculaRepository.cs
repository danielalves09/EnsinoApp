namespace EnsinoApp.Repositories.Matricula;

public interface IMatriculaRepository
{
    Task<List<Models.Entities.Matricula>> FindAllAsync();
    Task<Models.Entities.Matricula?> FindByIdAsync(int id);
    Task CreateAsync(Models.Entities.Matricula matricula);
    Task UpdateAsync(Models.Entities.Matricula matricula);
    Task DeleteAsync(int id);
}
