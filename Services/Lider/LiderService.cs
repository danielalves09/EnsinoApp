using EnsinoApp.Repositories.RelatorioSemanal;

namespace EnsinoApp.Services.Lider;

public class LiderService : ILiderService
{
    private readonly IRelatorioSemanalRepository _repository;

    public LiderService(IRelatorioSemanalRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Models.Entities.Turma>> ObterTurmasAsync(int idUsuario)
        => _repository.GetTurmasDoLiderAsync(idUsuario);

    public Task<List<Models.Entities.Matricula>> ObterMatriculasAsync(int idTurma)
        => _repository.GetMatriculasPorTurmaAsync(idTurma);

    public Task<List<Models.Entities.RelatorioSemanal>> ObterRelatoriosAsync(int idTurma)
        => _repository.GetRelatoriosAsync(idTurma);

    public Task CriarRelatorioAsync(Models.Entities.RelatorioSemanal relatorio)
        => _repository.AddAsync(relatorio);
}
