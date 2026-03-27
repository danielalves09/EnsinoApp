using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Agenda;

public enum StatusAula { Realizada, Hoje, Futura }

public class AgendaLicaoViewModel
{
    public int Id { get; set; }
    public int IdTurma { get; set; }
    public int NumeroLicao { get; set; }
    public string TituloLicao { get; set; } = null!;
    public DateTime DataAula { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public string? Local { get; set; }
    public string? Observacoes { get; set; }
    public bool LembreteEnviado { get; set; }
    public StatusAula StatusAula { get; set; }

    // Helpers para a view
    public string DiaSemanaLabel => DiaSemana switch
    {
        DayOfWeek.Sunday => "Domingo",
        DayOfWeek.Monday => "Segunda",
        DayOfWeek.Tuesday => "Terça",
        DayOfWeek.Wednesday => "Quarta",
        DayOfWeek.Thursday => "Quinta",
        DayOfWeek.Friday => "Sexta",
        DayOfWeek.Saturday => "Sábado",
        _ => "-"
    };

    public string StatusBadge => StatusAula switch
    {
        StatusAula.Realizada => "concluida",
        StatusAula.Hoje => "emandamento",
        StatusAula.Futura => "acomecar",
        _ => ""
    };

    public string StatusLabel => StatusAula switch
    {
        StatusAula.Realizada => "Realizada",
        StatusAula.Hoje => "Hoje",
        StatusAula.Futura => "Futura",
        _ => ""
    };
}

/// <summary>DTO retornado para o FullCalendar via JSON.</summary>
public class CalendarioEventoDto
{
    public string Title { get; set; } = null!;
    public string Start { get; set; } = null!;   // ISO 8601
    public string? Url { get; set; }
    public string Color { get; set; } = "#2563eb";
    public string TextColor { get; set; } = "#fff";
}