namespace EnsinoApp.Services.Email;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
    Task SendNovoUsuarioAsync(string toEmail, string nomeMarido, string nomeEsposa, string senha);
    Task SendInscricaoConfirmadaAsync(string toEmailMarido, string toEmailEsposa,
        string nomeMarido, string nomeEsposa,
        string nomeCurso, string nomeCampus,
        bool participaGC, string? nomeGC,
        DateTime dataInscricao);
}