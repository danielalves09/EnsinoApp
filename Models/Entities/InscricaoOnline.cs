using System;
using EnsinoApp.Models.Entities;
namespace EnsinoApp.Models.Entities;

public class InscricaoOnline
{
    public int Id { get; set; }

    // Dados do casal
    public string NomeMarido { get; set; } = null!;
    public string NomeEsposa { get; set; } = null!;
    public string TelefoneMarido { get; set; } = null!;
    public string TelefoneEsposa { get; set; } = null!;
    public string EmailMarido { get; set; } = null!;
    public string EmailEsposa { get; set; } = null!;

    // Datas de Nascimento

    public DateTime? DataNascimentoMarido { get; set; }
    public DateTime? DataNascimentoEsposa { get; set; }

    // Endereço
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }

    // Campus e Curso
    public int IdCampus { get; set; }
    public int IdCurso { get; set; }
    public Campus Campus { get; set; } = null!;
    public Curso Curso { get; set; } = null!;

    // GC
    public bool ParticipaGC { get; set; } = false;
    public string? NomeGC { get; set; }

    // Convidado por casal formando
    public bool Convidado { get; set; } = false;
    public string? NomeCasalConvidador { get; set; }

    /// <summary>Dia da semana preferencial escolhido pelo casal (ex: "Sábado").</summary>
    public string? DiaPreferencial { get; set; }

    // Controle da inscrição
    public DateTime DataInscricao { get; set; } = DateTime.Now;
    public bool Processada { get; set; } = false;
}
