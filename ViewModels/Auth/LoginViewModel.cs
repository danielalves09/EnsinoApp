using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email obrigatório")]
    [EmailAddress(ErrorMessage = "Informe um email válido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha obrigatória")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Lembrar-me")]
    public bool RememberMe { get; set; }
}
