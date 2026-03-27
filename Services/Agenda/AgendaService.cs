using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Agenda;
using EnsinoApp.Services.Email;
using EnsinoApp.Services.Util;
using EnsinoApp.ViewModels.Agenda;

namespace EnsinoApp.Services.Agenda;

public class AgendaService : IAgendaService
{
  private readonly IAgendaRepository _repository;
  private readonly IEmailService _emailService;

  private readonly ILogger<AgendaService> _logger;

  public AgendaService(
      IAgendaRepository repository,
      IEmailService emailService,
      ILogger<AgendaService> logger)
  {
    _repository = repository;
    _emailService = emailService;
    _logger = logger;
  }

  // ─── Geração automática da agenda ────────────────────────────────────────

  public async Task GerarAgendaAsync(Turma turma, IEnumerable<Models.Entities.Licao> licoes)
  {
    var licoesOrdenadas = licoes.OrderBy(l => l.NumeroSemana).ToList();

    if (!licoesOrdenadas.Any())
    {
      _logger.LogWarning("Turma {IdTurma}: nenhuma lição encontrada para gerar agenda.", turma.Id);
      return;
    }

    // Calcula a primeira ocorrência do DiaSemana a partir da DataInicio
    var primeiraData = ProximoDiaDaSemana(turma.DataInicio, turma.DiaSemana);

    var agenda = licoesOrdenadas.Select((licao, index) => new AgendaLicao
    {
      IdTurma = turma.Id,
      IdLicao = licao.Id,
      DataAula = primeiraData.AddDays(7 * index),
      DiaSemana = turma.DiaSemana,
      LembreteEnviado = false
    }).ToList();

    await _repository.CreateRangeAsync(agenda);

    _logger.LogInformation(
        "Agenda gerada para turma {IdTurma}: {Total} lições, de {Inicio} a {Fim}.",
        turma.Id, agenda.Count,
        agenda.First().DataAula.ToShortDateString(),
        agenda.Last().DataAula.ToShortDateString());
  }

  // ─── Consultas ────────────────────────────────────────────────────────────

  public async Task<List<AgendaLicaoViewModel>> GetAgendaTurmaAsync(int idTurma)
  {
    var itens = await _repository.GetByTurmaAsync(idTurma);
    var hoje = DateTime.Today;

    return itens.Select(a => new AgendaLicaoViewModel
    {
      Id = a.Id,
      IdTurma = a.IdTurma,
      NumeroLicao = a.Licao.NumeroSemana,
      TituloLicao = a.Licao.Titulo,
      DataAula = a.DataAula,
      DiaSemana = a.DiaSemana,
      Local = a.Local,
      Observacoes = a.Observacoes,
      LembreteEnviado = a.LembreteEnviado,
      StatusAula = a.DataAula.Date < hoje
            ? StatusAula.Realizada
            : a.DataAula.Date == hoje
                ? StatusAula.Hoje
                : StatusAula.Futura
    }).ToList();
  }

  public async Task<AgendaLicao?> FindByIdAsync(int id)
      => await _repository.FindByIdAsync(id);

  public async Task AtualizarLocalAsync(int id, string? local, string? observacoes)
  {
    var agenda = await _repository.FindByIdAsync(id)
        ?? throw new KeyNotFoundException($"AgendaLicao {id} não encontrada.");

    agenda.Local = local?.Trim();
    agenda.Observacoes = observacoes?.Trim();

    await _repository.UpdateAsync(agenda);
  }

  // ─── Lembretes ────────────────────────────────────────────────────────────

  public async Task ProcessarLembretesAsync()
  {
    // Busca aulas que acontecem AMANHÃ e cujo lembrete ainda não foi enviado
    var amanha = DateTime.Today.AddDays(1);
    var pendentes = await _repository.GetPendentesLembreteAsync(amanha);

    if (!pendentes.Any())
    {
      _logger.LogInformation("Lembretes: nenhuma aula amanhã ({Data}).", amanha.ToShortDateString());
      return;
    }

    _logger.LogInformation("Lembretes: {Total} aula(s) amanhã.", pendentes.Count);

    foreach (var item in pendentes)
    {
      var destinatarios = item.Turma.Matriculas
          .SelectMany(m => new[] { m.Casal.EmailConjuge1, m.Casal.EmailConjuge2 })
          .Where(e => !string.IsNullOrWhiteSpace(e))
          .Distinct()
          .ToList();

      if (!destinatarios.Any()) continue;

      var assunto = $"🗓 Lembrete: Aula de amanhã – {item.Licao.Titulo}";
      var corpo = MontarCorpoEmail(item);

      await _emailService.EnviarParaVariosAsync(destinatarios, assunto, corpo);
      await _repository.MarcarLembreteEnviadoAsync(item.Id);

      _logger.LogInformation(
          "Lembrete enviado | Turma {IdTurma} | Lição {Licao} | {Total} destinatários.",
          item.IdTurma, item.Licao.Titulo, destinatarios.Count);
    }
  }

  // ─── Helpers ──────────────────────────────────────────────────────────────

  /// <summary>
  /// Retorna a primeira ocorrência de <paramref name="diaSemana"/> a partir de
  /// <paramref name="dataInicio"/> (inclusive).
  /// </summary>
  private static DateTime ProximoDiaDaSemana(DateTime dataInicio, DayOfWeek diaSemana)
  {
    var diasAte = ((int)diaSemana - (int)dataInicio.DayOfWeek + 7) % 7;
    return dataInicio.Date.AddDays(diasAte);
  }

  private static string MontarCorpoEmail(AgendaLicao item)
  {
    var local = string.IsNullOrWhiteSpace(item.Local)
        ? "<em>Local não informado</em>"
        : $"<strong>{item.Local}</strong>";

    var obs = string.IsNullOrWhiteSpace(item.Observacoes)
        ? ""
        : $@"<p style='margin-top:10px;color:#555'><strong>Observações:</strong> {item.Observacoes}</p>";

    var lider = $"{item.Turma.Lider.NomeMarido} e {item.Turma.Lider.NomeEsposa}";
    return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head><meta charset='UTF-8'></head>
<body style='font-family:Nunito,Arial,sans-serif;background:#f8fafc;margin:0;padding:0'>
  <div style='max-width:560px;margin:40px auto;background:#fff;border-radius:12px;
              box-shadow:0 4px 18px rgba(0,0,0,.08);overflow:hidden'>

    <!-- Header -->
    <div style='background:linear-gradient(135deg,#1e40af,#2563eb);padding:28px 32px'>
      <p style='color:rgba(255,255,255,.75);font-size:13px;margin:0 0 4px'>CCVideira · Ensino</p>
      <h1 style='color:#fff;font-size:20px;margin:0'>🗓 Lembrete de Aula</h1>
    </div>

    <!-- Body -->
    <div style='padding:28px 32px'>
      <p style='color:#334155;font-size:15px;margin:0 0 20px'>
        Olá! Amanhã é dia de aula. Não esqueça! 😊
      </p>

      <table style='width:100%;border-collapse:collapse;font-size:14px'>
        <tr>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#64748b;width:40%'>
            📚 Lição
          </td>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#1e293b;font-weight:700'>
            {item.Licao.NumeroSemana}. {item.Licao.Titulo}
          </td>
        </tr>
        <tr>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#64748b'>
            📅 Data
          </td>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#1e293b;font-weight:700'>
            {item.DataAula:dddd, dd/MM/yyyy}
          </td>
        </tr>
        <tr>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#64748b'>
            📍 Local
          </td>
          <td style='padding:10px 0;border-bottom:1px solid #e2e8f0;color:#1e293b'>
            {local}
          </td>
        </tr>
        <tr>
          <td style='padding:10px 0;color:#64748b'>
            👤 Líderes
          </td>
          <td style='padding:10px 0;color:#1e293b'>
            {lider}
          </td>
        </tr>
      </table>

      {obs}

      <div style='margin-top:28px;padding:16px;background:#eff6ff;border-radius:8px;
                  border-left:4px solid #2563eb'>
        <p style='margin:0;color:#1d4ed8;font-size:13px'>
          💡 Em caso de dúvidas entre em contato com seus líderes.
        </p>
      </div>
    </div>

    <!-- Footer -->
    <div style='background:#f8fafc;border-top:1px solid #e2e8f0;padding:16px 32px;text-align:center'>
      <p style='color:#94a3b8;font-size:12px;margin:0'>
        EnsinoApp · CCVideira · Este é um email automático.
      </p>
    </div>
  </div>
</body>
</html>";
  }
}