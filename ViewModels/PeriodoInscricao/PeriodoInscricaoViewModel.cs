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

    // Status calculado para exibição
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
}