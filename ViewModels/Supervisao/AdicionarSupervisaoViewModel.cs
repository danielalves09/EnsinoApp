using System.ComponentModel.DataAnnotations;
using EnsinoApp.Models.Entities;

namespace EnsinoApp.ViewModels.Supervisao;

public class AdicionarSupervisaoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;

    [Display(Name = "Campus")]
    public int IdCampus { get; set; }

}