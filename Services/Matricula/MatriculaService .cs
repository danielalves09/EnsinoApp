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

    public async Task<List<Models.Entities.Matricula>> FindByTurmaAsync(int idTurma)
    {
        return await _repository.FindByTurmaAsync(idTurma);
    }

    public Task CreateAsync(Models.Entities.Matricula matricula) => _repository.CreateAsync(matricula);
    public Task UpdateAsync(Models.Entities.Matricula matricula) => _repository.UpdateAsync(matricula);
    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    public int ContarAtivas()
    {
        return _repository.ContarAtivas();
    }

    public void MatricularCasal(int idCasal, int idTurma)
    {
        if (_repository.ExisteMatriculaAtiva(idCasal, idTurma))
            throw new Exception("Casal já possui matrícula ativa nesta turma.");

        var matricula = new Models.Entities.Matricula
        {
            IdCasal = idCasal,
            IdTurma = idTurma,
            DataMatricula = DateTime.Now
        };

        _repository.CreateAsync(matricula);
    }

    public async Task<IEnumerable<(string Curso, int Total)>> GetMatriculasPorCursoAsync()
    {
        return await _repository.GetMatriculasPorCursoAsync();
    }

    public async Task<bool> PodeConcluirCursoAsync(int idMatricula)
    {
        var totalRelatorios = await _repository.GetTotalRelatoriosAsync(idMatricula);
        var totalLicoes = await _repository.GetTotalLicoesDoCursoAsync(idMatricula);

        if (totalLicoes == 0)
            return false;

        return totalRelatorios >= totalLicoes;
    }
    public async Task ConcluirCursoAsync(int idMatricula)
    {
        var podeConcluir = await PodeConcluirCursoAsync(idMatricula);

        if (!podeConcluir)
            throw new Exception("Matrícula ainda não atende os critérios para conclusão.");

        await _repository.AtualizarConclusaoAsync(idMatricula);
    }
}