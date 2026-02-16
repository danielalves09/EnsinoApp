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
    Task<IEnumerable<(string Curso, int Total)>> GetMatriculasPorCursoAsync();
    Task<bool> PodeConcluirCursoAsync(int idMatricula);
    Task ConcluirCursoAsync(int idMatricula);

    Task<List<Models.Entities.Matricula>> GetConcluidasSemCertificadoAsync();

    Task<int> CountMatriculasConcluidasSemCertificadoAsync();

    Task<Models.Entities.Matricula> GetByCodigoValidacaoAsync(string codigo);
}