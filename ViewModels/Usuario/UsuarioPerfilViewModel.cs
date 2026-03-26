using Microsoft.AspNetCore.Identity;

namespace EnsinoApp.ViewModels.Usuario;

public class UsuarioPerfilViewModel
{
    public string Nome { get; set; } = null!;
    public string? NomeMarido { get; set; } = null!;
    public string? NomeEsposa { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string? FotoPerfilUrl { get; set; }

    public string Role { get; set; } = null!;
    public string? Campus { get; set; }
    public string? Supervisao { get; set; }

    public IFormFile Foto { get; set; }

    public string? SenhaAtual { get; set; }
    public string? NovaSenha { get; set; }
    public string? ConfirmarSenha { get; set; }

}