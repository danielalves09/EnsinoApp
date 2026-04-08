using EnsinoApp.Models.Entities;

namespace EnsinoApp.Services.PeriodoInscricao;

public interface IPeriodoInscricaoService
{
    Task<List<Models.Entities.PeriodoInscricao>> FindByCursoAsync(int idCurso);
    Task<Models.Entities.PeriodoInscricao?> FindByIdAsync(int id);

    Task<Models.Entities.PeriodoInscricao?> FindAtivoAsync(int idCurso, int idCampus);
    Task<List<Models.Entities.PeriodoInscricao>> FindTodosAbertosAsync();

    Task<Models.Entities.PeriodoInscricao> CreateAsync(Models.Entities.PeriodoInscricao periodo);
    Task<Models.Entities.PeriodoInscricao> UpdateAsync(Models.Entities.PeriodoInscricao periodo);
    Task ToggleAtivoAsync(int id);
    Task DeleteAsync(int id);

    /// <summary>
    /// Verifica se ainda há vagas e o período está aberto e,
    /// caso sim, incrementa VagasOcupadas. Lança InvalidOperationException caso contrário.
    /// </summary>
    Task ReservarVagaAsync(int idCurso, int idCampus);
}