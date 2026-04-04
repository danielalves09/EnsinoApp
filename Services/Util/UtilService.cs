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

    public string DiaSemanaLabel(string diaSemana)
    {
        if (Enum.TryParse<DayOfWeek>(diaSemana, true, out var d))
        {
            return d switch
            {
                DayOfWeek.Sunday => "Domingo",
                DayOfWeek.Monday => "Segunda-feira",
                DayOfWeek.Tuesday => "Terça-feira",
                DayOfWeek.Wednesday => "Quarta-feira",
                DayOfWeek.Thursday => "Quinta-feira",
                DayOfWeek.Friday => "Sexta-feira",
                DayOfWeek.Saturday => "Sábado",
                _ => "-"
            };
        }

        return "-";
    }
}