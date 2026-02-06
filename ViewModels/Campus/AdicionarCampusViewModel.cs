namespace EnsinoApp.ViewModels.campus;

public class AdicionarCampusViewModel
{

    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;

    // Endereço
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }

}