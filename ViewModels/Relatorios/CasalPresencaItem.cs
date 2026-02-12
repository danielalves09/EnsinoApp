using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Relatorios;

public class CasalPresencaItem
{
    public int IdMatricula { get; set; }
    public string Casal { get; set; } = null!;
    public StatusPresenca Presenca { get; set; }
}