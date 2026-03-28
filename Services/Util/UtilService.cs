namespace EnsinoApp.Services.Util;


public class UtilService : IUtilService
{
    public string GetNomeReduzido(string nome1, string nome2)
    {

        return $"{GetPrimeiroNome(nome1)} e {GetPrimeiroNome(nome2)}";
    }

    public string GetNomeSobrenome(string nome1, string nome2)
    {
        return $"{GetPrimeiroNome(nome1)} {GetSobrenome(nome1)} e {GetPrimeiroNome(nome2)} {GetSobrenome(nome2)}";
    }

    public string GetPrimeiroNome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        if (partes.Length >= 2)
            return $"{partes[0]}"; // Primeiro nome
        return partes[0];
    }

    private string GetSobrenome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        if (partes.Length >= 2)
            return $"{partes[partes.Length - 1]}"; // Sobrenome
        return partes[0];
    }
}