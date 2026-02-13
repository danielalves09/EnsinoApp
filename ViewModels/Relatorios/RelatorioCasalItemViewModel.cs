using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Relatorios;

public class RelatorioCasalItemViewModel
{
    public int IdMatricula { get; set; }
    public string NomeCasal { get; set; } = string.Empty;

    public StatusPresenca Presenca { get; set; }
    public string? Observacoes { get; set; }
}