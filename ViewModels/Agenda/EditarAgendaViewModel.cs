using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Agenda;

public class EditarAgendaViewModel
{
    public int Id { get; set; }
    public int IdTurma { get; set; }
    public int NumeroLicao { get; set; }
    public string TituloLicao { get; set; } = null!;
    public DateTime DataAula { get; set; }

    [MaxLength(300, ErrorMessage = "Máximo 300 caracteres.")]
    public string? Local { get; set; }

    [MaxLength(1000, ErrorMessage = "Máximo 1000 caracteres.")]
    public string? Observacoes { get; set; }
}