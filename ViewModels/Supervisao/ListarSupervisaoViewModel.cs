using EnsinoApp.Models.Entities;

namespace EnsinoApp.ViewModels.Supervisao;

public class ListarSupervisaoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public int CampusId { get; set; }
    public string NomeCampus { get; set; } = string.Empty;

}