namespace EnsinoApp.Services.Email;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string assunto, string corpoHtml);
    Task EnviarParaVariosAsync(IEnumerable<string> destinatarios, string assunto, string corpoHtml);
}