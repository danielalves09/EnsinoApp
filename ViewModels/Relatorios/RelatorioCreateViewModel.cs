using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.ViewModels.Relatorios;

public class RelatorioCreateViewModel
{
    public int IdTurma { get; set; }
    public int IdCurso { get; set; }
    public int IdLicao { get; set; }

    public DateTime DataLicao { get; set; }

    // Dropdown
    public List<SelectListItem> Licoes { get; set; } = new();

    // Relatórios por casal
    public List<RelatorioCasalItemViewModel> Casais { get; set; } = new();
}