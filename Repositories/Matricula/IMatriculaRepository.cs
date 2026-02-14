namespace EnsinoApp.Repositories.Matricula;

public interface IMatriculaRepository
{
    Task<List<Models.Entities.Matricula>> FindAllAsync();
    Task<Models.Entities.Matricula?> FindByIdAsync(int id);
    Task<List<Models.Entities.Matricula>> FindByTurmaAsync(int idTurma);
    Task CreateAsync(Models.Entities.Matricula matricula);
    Task UpdateAsync(Models.Entities.Matricula matricula);
    Task DeleteAsync(int id);

    int ContarAtivas();
    bool ExisteMatriculaAtiva(int idCasal, int idTurma);

    bool ExisteMatriculaAtivaPorCasal(int idCasal);
    Task<IEnumerable<(string Curso, int Total)>> GetMatriculasPorCursoAsync();

    Task<int> GetTotalRelatoriosAsync(int idMatricula);
    Task<int> GetTotalLicoesDoCursoAsync(int idMatricula);
    Task AtualizarConclusaoAsync(int idMatricula);
    Task<List<Models.Entities.Matricula>> GetConcluidasSemCertificadoAsync();

    Task<int> CountConcluidasSemCertificadoAsync();

}
