namespace EnsinoApp.ViewModels.campus;

public class AdicionarCampusViewModel
{

    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;

    // Endereço
    public string Rua { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Cep { get; set; } = null!;

}