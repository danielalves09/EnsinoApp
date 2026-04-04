using EnsinoApp.Models.Entities;
using EnsinoApp.ViewModels.Agenda;

namespace EnsinoApp.Services.Agenda;

public interface IAgendaService
{
    /// <summary>
    /// Gera e persiste automaticamente a agenda de uma turma recém-criada.
    /// Calcula as datas com base em DiaSemana e DataInicio da turma.
    /// </summary>
    Task GerarAgendaAsync(Turma turma, IEnumerable<Models.Entities.Licao> licoes);

    /// <summary>Retorna a agenda completa de uma turma para exibição na view.</summary>
    Task<List<AgendaLicaoViewModel>> GetAgendaTurmaAsync(int idTurma);

    /// <summary>Retorna um item da agenda para edição.</summary>
    Task<AgendaLicao?> FindByIdAsync(int id);

    /// <summary>Atualiza local e observações de um item da agenda.</summary>
    Task AtualizarLocalAsync(int id, DateTime dataAula, string? local, string? observacoes);

    /// <summary>Processa e envia os lembretes do dia (chamado pelo BackgroundService).</summary>
    Task ProcessarLembretesAsync();
}