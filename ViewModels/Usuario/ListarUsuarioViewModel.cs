namespace EnsinoApp.ViewModels.Usuario;

public class ListarUsuarioViewModel
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    // public string NomeCompleto
    //     => $"{NomeMarido} e {NomeEsposa}";

    public string NomeCompleto
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(NomeMarido) && !string.IsNullOrWhiteSpace(NomeEsposa))
                return $"{NomeMarido} e {NomeEsposa}";
            else if (!string.IsNullOrWhiteSpace(NomeMarido))
                return NomeMarido;
            else if (!string.IsNullOrWhiteSpace(NomeEsposa))
                return NomeEsposa;
            else
                return "Nome não definido";
        }
    }

    public string NomeMarido { get; set; } = null!;
    public string NomeEsposa { get; set; } = null!;

    public string Campus { get; set; } = null!;
    public string? Supervisao { get; set; }

    public string TipoUsuario { get; set; } = null!;


    public bool Ativo { get; set; }
}