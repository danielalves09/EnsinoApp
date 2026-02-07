using System.ComponentModel.DataAnnotations;
using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.ViewModels.Usuario;

public class AdicionarUsuarioViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Senha", ErrorMessage = "As senhas não conferem")]

    [Display(Name = "Confirmar Senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Required]
    public int IdCampus { get; set; }

    public string NomeCampus { get; set; } = string.Empty; // Para mostrar no input

    [Display(Name = "Nome do Marido")]
    public string? NomeMarido { get; set; }

    [Display(Name = "Nome da Esposa")]
    public string? NomeEsposa { get; set; }

    public int? IdSupervisao { get; set; }
    public string NomeSupervisao { get; set; } = string.Empty;

    [Required]
    public bool Ativo { get; set; } = true;

    [Required]
    [Display(Name = "Perfil do Usuario")]
    public string Role { get; set; } = string.Empty;

    public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>()
 {
     new SelectListItem(){ Value = "Admin",Text = "Admin"},
     new SelectListItem(){ Value = "Pastor",Text = "Pastor"},
     new SelectListItem(){ Value = "Coordenador",Text = "Coordenador"},
     new SelectListItem(){ Value = "Lider",Text = "Lider"}

 };

}