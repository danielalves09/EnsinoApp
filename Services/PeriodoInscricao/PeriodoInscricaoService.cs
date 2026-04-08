using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.PeriodoInscricao;

namespace EnsinoApp.Services.PeriodoInscricao;

public class PeriodoInscricaoService : IPeriodoInscricaoService
{
    private readonly IPeriodoInscricaoRepository _repository;

    public PeriodoInscricaoService(IPeriodoInscricaoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Models.Entities.PeriodoInscricao>> FindByCursoAsync(int idCurso)
        => _repository.FindByCursoAsync(idCurso);

    public Task<Models.Entities.PeriodoInscricao?> FindByIdAsync(int id)
        => _repository.FindByIdAsync(id);

    public Task<Models.Entities.PeriodoInscricao?> FindAtivoAsync(int idCurso, int idCampus)
        => _repository.FindAtivoAsync(idCurso, idCampus);

    public Task<List<Models.Entities.PeriodoInscricao>> FindTodosAbertosAsync()
        => _repository.FindTodosAbertosAsync();

    public Task<Models.Entities.PeriodoInscricao> CreateAsync(Models.Entities.PeriodoInscricao periodo)
        => _repository.CreateAsync(periodo);

    public Task<Models.Entities.PeriodoInscricao> UpdateAsync(Models.Entities.PeriodoInscricao periodo)
        => _repository.UpdateAsync(periodo);

    public async Task ToggleAtivoAsync(int id)
    {
        var periodo = await _repository.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("Período não encontrado.");

        // Para evitar dois períodos ativos para o mesmo curso+campus,
        // desativa todos os outros antes de ativar este.
        if (!periodo.Ativo)
        {
            var existentes = await _repository.FindByCursoAsync(periodo.IdCurso);
            foreach (var outro in existentes.Where(p => p.Id != id && p.IdCampus == periodo.IdCampus && p.Ativo))
            {
                outro.Ativo = false;
                await _repository.UpdateAsync(outro);
            }
        }

        // Reatribui (FindByIdAsync é AsNoTracking, então precisamos de nova instância)
        var fresh = await _repository.FindByIdAsync(id)!;
        fresh!.Ativo = !fresh.Ativo;
        await _repository.UpdateAsync(fresh);
    }

    public Task DeleteAsync(int id)
        => _repository.DeleteAsync(id);

    public async Task ReservarVagaAsync(int idCurso, int idCampus)
    {
        var periodo = await _repository.FindAtivoAsync(idCurso, idCampus);

        if (periodo is null)
            throw new InvalidOperationException(
                "Não há período de inscrição aberto para este curso e campus.");

        if (periodo.VagasRestantes <= 0)
            throw new InvalidOperationException(
                "Todas as vagas para este curso já foram preenchidas.");

        await _repository.IncrementarVagaAsync(periodo.Id);
    }
}