using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.PeriodoInscricao;

public class PeriodoInscricaoViewModel
{
    public int Id { get; set; }
    public int IdCurso { get; set; }
    public int IdCampus { get; set; }
    public string NomeCampus { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;

    public DateTime DataAbertura { get; set; }
    public DateTime DataEncerramento { get; set; }

    public int VagasTotal { get; set; }
    public int VagasOcupadas { get; set; }
    public int VagasRestantes => VagasTotal - VagasOcupadas;

    public bool Ativo { get; set; }

    /// <summary>Dias disponíveis separados por vírgula, ex: "Monday,Saturday"</summary>
    public string? DiasDisponiveis { get; set; }

    /// <summary>Lista de dias para exibição amigável na view.</summary>
    public List<string> ListaDiasNomes =>
        DiasDisponivelHelper.ParseNomes(DiasDisponiveis);

    public string StatusLabel
    {
        get
        {
            if (!Ativo) return "Inativo";
            var agora = DateTime.Now;
            if (agora < DataAbertura) return "Agendado";
            if (agora > DataEncerramento) return "Encerrado";
            if (VagasRestantes <= 0) return "Sem vagas";
            return "Aberto";
        }
    }

    public string StatusBadge => StatusLabel switch
    {
        "Aberto" => "ativo",
        "Agendado" => "acomecar",
        "Encerrado" => "cancelada",
        "Sem vagas" => "pendente",
        _ => "inativo"
    };
}

/// <summary>Formulário de criação / edição de período.</summary>
public class PeriodoInscricaoFormViewModel
{
    public int Id { get; set; }

    [Required]
    public int IdCurso { get; set; }
    public string NomeCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o campus")]
    [Display(Name = "Campus")]
    public int IdCampus { get; set; }
    public string NomeCampus { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data de abertura")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Abertura das inscrições")]
    public DateTime DataAbertura { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Informe a data de encerramento")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Encerramento das inscrições")]
    public DateTime DataEncerramento { get; set; } = DateTime.Today.AddDays(30);

    [Required(ErrorMessage = "Informe o número de vagas")]
    [Range(1, 9999, ErrorMessage = "O número de vagas deve ser entre 1 e 9999")]
    [Display(Name = "Total de vagas")]
    public int VagasTotal { get; set; } = 30;

    public bool Ativo { get; set; } = false;

    /// <summary>
    /// Dias da semana selecionados pelo administrador.
    /// Armazenados como string separada por vírgula, ex: "Saturday,Sunday".
    /// </summary>
    [Display(Name = "Dias disponíveis")]
    public List<string> DiasDisponiveis { get; set; } = new();

    /// <summary>Converte a lista para a string que vai no banco.</summary>
    public string DiasDisponiveisStr =>
        DiasDisponiveis != null && DiasDisponiveis.Any()
            ? string.Join(",", DiasDisponiveis.Where(d => !string.IsNullOrWhiteSpace(d)))
            : string.Empty;
}

/// <summary>Helper estático para conversão dos dias.</summary>
public static class DiasDisponivelHelper
{
    private static readonly Dictionary<string, string> _ptBr = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Sunday",    "Domingo"       },
        { "Monday",    "Segunda-feira" },
        { "Tuesday",   "Terça-feira"   },
        { "Wednesday", "Quarta-feira"  },
        { "Thursday",  "Quinta-feira"  },
        { "Friday",    "Sexta-feira"   },
        { "Saturday",  "Sábado"        },
    };

    /// <summary>Ordem de exibição: começa na segunda, vai até domingo.</summary>
    public static readonly List<(string Valor, string Label)> TodosOsDias = new()
    {
        ("Monday",    "Segunda-feira"),
        ("Tuesday",   "Terça-feira"  ),
        ("Wednesday", "Quarta-feira" ),
        ("Thursday",  "Quinta-feira" ),
        ("Friday",    "Sexta-feira"  ),
        ("Saturday",  "Sábado"       ),
        ("Sunday",    "Domingo"      ),
    };

    public static string ParaPtBr(string? dia) =>
        !string.IsNullOrWhiteSpace(dia) && _ptBr.TryGetValue(dia, out var label) ? label : dia ?? "";

    public static List<string> ParseNomes(string? diasStr)
    {
        if (string.IsNullOrWhiteSpace(diasStr)) return new List<string>();
        return diasStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(d => ParaPtBr(d.Trim()))
                      .ToList();
    }

    public static List<string> ParseValores(string? diasStr)
    {
        if (string.IsNullOrWhiteSpace(diasStr)) return new List<string>();
        return diasStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(d => d.Trim())
                      .ToList();
    }
}
