using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.Cursos;

public class CursoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O campus é obrigatório")]
    public int IdCampus { get; set; }

    [Display(Name = "Campus")]
    public string NomeCampus { get; set; } = null!;

    [Required(ErrorMessage = "Nome do curso é obrigatório")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Descrição do curso é obrigatória")]
    public string Descricao { get; set; } = null!;

    public bool Ativo { get; set; } = true;
}