namespace EnsinoApp.Models.Entities;

public class AgendaLicao
{
    public int Id { get; set; }
    public int IdTurma { get; set; }
    public int IdLicao { get; set; }

    /// <summary>Calculada automaticamente ao criar a turma.</summary>
    public DateTime DataAula { get; set; }

    /// <summary>Dia da semana herdado da turma (armazenado para exibição e recalculo).</summary>
    public DayOfWeek DiaSemana { get; set; }

    /// <summary>Local informado pelo líder semanalmente (opcional).</summary>
    public string? Local { get; set; }

    /// <summary>Observações adicionais do líder.</summary>
    public string? Observacoes { get; set; }

    /// <summary>Flag para evitar reenvio do lembrete.</summary>
    public bool LembreteEnviado { get; set; } = false;

    // Navegação
    public Turma Turma { get; set; } = null!;
    public Licao Licao { get; set; } = null!;
}