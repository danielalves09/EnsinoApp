namespace EnsinoApp.Services.Lider;

public interface ILiderService
{
    Task<List<Models.Entities.Turma>> ObterTurmasAsync(int idUsuario);
    Task<List<Models.Entities.Matricula>> ObterMatriculasAsync(int idTurma);
    Task<List<Models.Entities.RelatorioSemanal>> ObterRelatoriosAsync(int idTurma);
    Task CriarRelatorioAsync(Models.Entities.RelatorioSemanal relatorio);
}
