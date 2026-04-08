using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.PeriodoInscricao;

public interface IPeriodoInscricaoRepository
{
    Task<List<Models.Entities.PeriodoInscricao>> FindAllAsync();
    Task<List<Models.Entities.PeriodoInscricao>> FindByCursoAsync(int idCurso);
    Task<Models.Entities.PeriodoInscricao?> FindByIdAsync(int id);

    /// <summary>Retorna o período ativo e aberto para um curso+campus específico.</summary>
    Task<Models.Entities.PeriodoInscricao?> FindAtivoAsync(int idCurso, int idCampus);

    /// <summary>Retorna todos os pares curso+campus que ainda estão com inscrições abertas.</summary>
    Task<List<Models.Entities.PeriodoInscricao>> FindTodosAbertosAsync();

    Task<Models.Entities.PeriodoInscricao> CreateAsync(Models.Entities.PeriodoInscricao periodo);
    Task<Models.Entities.PeriodoInscricao> UpdateAsync(Models.Entities.PeriodoInscricao periodo);
    Task DeleteAsync(int id);

    /// <summary>Incrementa VagasOcupadas em 1 de forma segura (UPDATE ... SET VagasOcupadas = VagasOcupadas + 1).</summary>
    Task IncrementarVagaAsync(int id);
}