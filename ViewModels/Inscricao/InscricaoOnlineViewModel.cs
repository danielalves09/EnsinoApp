using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Inscricao;

public class InscricaoOnlineViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Nome Marido")]
    public string NomeMarido { get; set; } = null!;
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Nome Esposa")]
    public string NomeEsposa { get; set; } = null!;
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Telefone Marido")]
    public string TelefoneMarido { get; set; } = null!;
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Telefone Esposa")]
    public string TelefoneEsposa { get; set; } = null!;
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Email Marido")]
    public string EmailMarido { get; set; } = null!;
    [Required(ErrorMessage = "Campo obrigatório")]
    [DisplayName("Email Esposa")]
    public string EmailEsposa { get; set; } = null!;
    public string Rua { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string? Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Cep { get; set; } = null!;
    [DisplayName("Qual Campus você pertence?")]
    public int IdCampus { get; set; }
    [DisplayName("Deseja se inscrever para qual Curso?")]
    public int IdCurso { get; set; }
    public bool ParticipaGC { get; set; }
    [DisplayName("Nome do GC")]
    public string? NomeGC { get; set; }
    public DateTime DataInscricao { get; set; }
    public bool Processada { get; set; }
}
