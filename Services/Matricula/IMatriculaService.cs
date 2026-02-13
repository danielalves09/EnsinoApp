namespace EnsinoApp.Services.Matricula;

public interface IMatriculaService
{
    Task<List<Models.Entities.Matricula>> FindAllAsync();
    Task<Models.Entities.Matricula?> FindByIdAsync(int id);
    Task<List<Models.Entities.Matricula>> FindByTurmaAsync(int idTurma);
    Task CreateAsync(Models.Entities.Matricula matricula);
    Task UpdateAsync(Models.Entities.Matricula matricula);
    Task DeleteAsync(int id);

    int ContarAtivas();
    void MatricularCasal(int idCasal, int idTurma);
}