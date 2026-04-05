using System.ComponentModel.DataAnnotations;
using EnsinoApp.Models.Enums;

namespace EnsinoApp.ViewModels.Casal;

public class AdicionarCasalViewModel
{
    public int Id { get; set; }

    // Homem
    [Required(ErrorMessage = "Nome do cônjuge 1 é obrigatório")]
    [Display(Name = "Nome do Cônjuge 1")]
    public string NomeConjuge1 { get; set; } = null!;

    [Required(ErrorMessage = "Telefone do cônjuge 1 é obrigatório")]
    [Display(Name = "Telefone do Cônjuge 1")]
    public string TelefoneConjuge1 { get; set; } = null!;

    [Required(ErrorMessage = "Email do cônjuge 1 é obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um email válido")]
    [Display(Name = "Email do Cônjuge 1")]
    public string EmailConjuge1 { get; set; } = null!;

    // Mulher
    [Required(ErrorMessage = "Nome do cônjuge 2 é obrigatório")]
    [Display(Name = "Nome do Cônjuge 2")]
    public string NomeConjuge2 { get; set; } = null!;

    [Required(ErrorMessage = "Telefone do cônjuge 2 é obrigatório")]
    [Display(Name = "Telefone do Cônjuge 2")]
    public string TelefoneConjuge2 { get; set; } = null!;

    [Required(ErrorMessage = "Email do cônjuge 2 é obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um email válido")]
    [Display(Name = "Email do Cônjuge 2")]
    public string EmailConjuge2 { get; set; } = null!;

    // Vínculo
    [Required(ErrorMessage = "Campus é obrigatório")]
    [Display(Name = "Campus")]
    public int IdCampus { get; set; }
    public string NomeCampus { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public StatusCasal Status { get; set; } = StatusCasal.Ativo;

    // Endereço
    [Display(Name = "CEP")]
    public string? Cep { get; set; }

    [Display(Name = "Rua")]
    public string? Rua { get; set; }

    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

    [Display(Name = "Cidade")]
    public string? Cidade { get; set; }

    [Display(Name = "Estado")]
    public string? Estado { get; set; }
}