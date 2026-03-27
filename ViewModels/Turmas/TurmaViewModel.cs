using System.ComponentModel.DataAnnotations;
using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Turmas;


public class TurmaViewModel
{
    public int Id { get; set; }

    [Required]
    public int IdCurso { get; set; }
    public string NomeCurso { get; set; } = string.Empty;

    [Required]
    public int IdCampus { get; set; }
    public string NomeCampus { get; set; } = string.Empty;

    [Required]
    public int IdLider { get; set; }
    public string NomeLider { get; set; } = string.Empty;

    public string? imgLider { get; set; } = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataInicio { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataFim { get; set; }

    public StatusTurma Status { get; set; } = StatusTurma.Acomecar;
}