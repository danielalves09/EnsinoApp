using EnsinoApp.Services.Agenda;

namespace EnsinoApp.Services.Lembrete;

/// <summary>
/// Serviço em segundo plano que verifica diariamente às 8h
/// quais aulas acontecem amanhã e envia os lembretes por email.
/// </summary>
public class LembreteBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LembreteBackgroundService> _logger;

    // Hora do dia em que os lembretes são processados
    private static readonly TimeSpan HorarioExecucao = new(8, 0, 0);

    public LembreteBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<LembreteBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LembreteBackgroundService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var agora = DateTime.Now;
            var proximaExecucao = DateTime.Today.Add(HorarioExecucao);

            // Se já passou das 8h hoje, agenda para amanhã
            if (agora >= proximaExecucao)
                proximaExecucao = proximaExecucao.AddDays(1);

            var delay = proximaExecucao - agora;

            _logger.LogInformation(
                "Próximo envio de lembretes em: {ProximaExecucao} (daqui a {Horas:F1}h).",
                proximaExecucao, delay.TotalHours);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await ProcessarAsync(stoppingToken);
        }

        _logger.LogInformation("LembreteBackgroundService encerrado.");
    }

    private async Task ProcessarAsync(CancellationToken stoppingToken)
    {
        try
        {
            // IAgendaService é Scoped — precisa de um escopo manual dentro do Singleton
            using var scope = _scopeFactory.CreateScope();
            var agendaService = scope.ServiceProvider.GetRequiredService<IAgendaService>();

            _logger.LogInformation("Processando lembretes...");
            await agendaService.ProcessarLembretesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar lembretes.");
        }
    }
}