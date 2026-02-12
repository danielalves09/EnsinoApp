

namespace EnsinoApp.Repositories.RelatorioSemanal;

public interface IRelatorioSemanalRepository
{
    Task<List<Models.Entities.Turma>> GetTurmasDoLiderAsync(int idUsuario);
    Task<List<Models.Entities.Matricula>> GetMatriculasPorTurmaAsync(int idTurma);
    Task<List<Models.Entities.RelatorioSemanal>> GetRelatoriosAsync(int idTurma);
    Task AddAsync(Models.Entities.RelatorioSemanal relatorio);
}