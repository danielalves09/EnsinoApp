namespace EnsinoApp.ViewModels.InscricaoOnline;

public class InscricaoOnlineViewModel
{
    public int Id { get; set; }
    public string NomeMarido { get; set; } = null!;
    public string NomeEsposa { get; set; } = null!;
    public string TelefoneMarido { get; set; } = null!;
    public string TelefoneEsposa { get; set; } = null!;
    public string EmailMarido { get; set; } = null!;
    public string EmailEsposa { get; set; } = null!;
    public string Rua { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string Cep { get; set; } = null!;
    public int IdCampus { get; set; }
    public string NomeCampus { get; set; } = null!;
    public int IdCurso { get; set; }
    public string NomeCurso { get; set; } = null!;
    public bool ParticipaGC { get; set; }
    public string? NomeGC { get; set; }
    public DateTime DataInscricao { get; set; }
    public bool Processada { get; set; }
}
