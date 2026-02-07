namespace EnsinoApp.ViewModels.Usuario;

public class ListarUsuarioViewModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string NomeCompleto
        => $"{NomeMarido} e {NomeEsposa}";

    public string NomeMarido { get; set; } = null!;
    public string NomeEsposa { get; set; } = null!;

    public string Campus { get; set; } = null!;
    public string? Supervisao { get; set; }

    public bool Ativo { get; set; }
}