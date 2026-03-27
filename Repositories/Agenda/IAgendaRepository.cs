using EnsinoApp.Models.Entities;

namespace EnsinoApp.Repositories.Agenda;

public interface IAgendaRepository
{
    /// <summary>Retorna toda a agenda de uma turma, ordenada por data.</summary>
    Task<List<AgendaLicao>> GetByTurmaAsync(int idTurma);

    /// <summary>Busca uma entrada específica por Id.</summary>
    Task<AgendaLicao?> FindByIdAsync(int id);

    /// <summary>Retorna entradas do dia seguinte com lembrete ainda não enviado.</summary>
    Task<List<AgendaLicao>> GetPendentesLembreteAsync(DateTime dataAlvo);

    /// <summary>Cria todas as entradas da agenda em lote (chamado ao criar a turma).</summary>
    Task CreateRangeAsync(IEnumerable<AgendaLicao> agenda);

    /// <summary>Atualiza local/observações de uma entrada.</summary>
    Task UpdateAsync(AgendaLicao agenda);

    /// <summary>Marca o lembrete como enviado.</summary>
    Task MarcarLembreteEnviadoAsync(int idAgenda);

    /// <summary>Remove toda a agenda de uma turma (útil se o curso mudar).</summary>
    Task DeleteByTurmaAsync(int idTurma);
}