using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Agenda;

public class EditarAgendaViewModel
{
    public int Id { get; set; }
    public int IdTurma { get; set; }
    public int NumeroLicao { get; set; }
    public string TituloLicao { get; set; } = null!;
    public DateTime DataAula { get; set; }

    public string? Local { get; set; }
    public string? Observacoes { get; set; }
}